import React, { useCallback, useEffect, useRef, useState } from 'react';
import {
  ActivityIndicator,
  FlatList,
  RefreshControl,
  StyleSheet,
  Text,
  View,
} from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { ClusterRow } from '../components/ClusterRow';
import {
  ClusteringPanel,
  DEFAULT_EPSILON,
  DEFAULT_MIN_POINTS,
} from '../components/ClusteringPanel';
import { DashboardHeader } from '../components/DashboardHeader';
import { DebrisRow } from '../components/DebrisRow';
import { ScatterChart } from '../components/ScatterChart';
import { SegmentTabs } from '../components/SegmentTabs';
import { usePagination } from '../hooks/usePagination';
import { useResponsive } from '../hooks/useResponsive';
import {
  fetchAllClustersForChart,
  fetchAllDebrisForChart,
  getAlerts,
  getClustersPage,
  getDebrisPage,
  runClustering,
  runIngestion,
} from '../services/api';
import { colors } from '../theme/colors';
import { spacing } from '../theme/spacing';
import { fontFamily } from '../theme/typography';
import type { ClusterDto, ClusteringResultDto, DebrisDto } from '../types/api';

type Segment = 'clusters' | 'debris';

export function DashboardScreen() {
  const { isDesktop, contentMaxWidth } = useResponsive();
  const [segment, setSegment] = useState<Segment>('clusters');

  const [chartOverviewDebris, setChartOverviewDebris] = useState<DebrisDto[]>([]);
  const [chartOverviewClusters, setChartOverviewClusters] = useState<ClusterDto[]>([]);

  const [chartExperimentDebris, setChartExperimentDebris] = useState<DebrisDto[]>([]);
  const [chartExperimentClusters, setChartExperimentClusters] = useState<ClusterDto[]>([]);

  const [alertCount, setAlertCount] = useState(0);
  const [updating, setUpdating] = useState(false);

  const clusteringParams = useRef({ epsilon: DEFAULT_EPSILON, minPoints: DEFAULT_MIN_POINTS });

  const clusterPagination = usePagination(getClustersPage);
  const debrisPagination = usePagination(getDebrisPage);

  const activePagination = segment === 'clusters' ? clusterPagination : debrisPagination;

  const loadOverviewChart = useCallback(async () => {
    const [debris, clusters] = await Promise.all([
      fetchAllDebrisForChart(),
      fetchAllClustersForChart(),
    ]);
    setChartOverviewDebris(debris);
    setChartOverviewClusters(clusters);
  }, []);

  const loadExperimentChart = useCallback(async () => {
    const [debris, clusters] = await Promise.all([
      fetchAllDebrisForChart(),
      fetchAllClustersForChart(),
    ]);
    setChartExperimentDebris(debris);
    setChartExperimentClusters(clusters);
  }, []);

  const loadAlerts = useCallback(async () => {
    const alerts = await getAlerts();
    setAlertCount(alerts.filter(a => !a.isAcknowledged).length);
  }, []);

  const loadCatalog = useCallback(async () => {
    await Promise.all([clusterPagination.loadFirst(), debrisPagination.loadFirst()]);
  }, [clusterPagination, debrisPagination]);

  const loadInitial = useCallback(async () => {
    await Promise.all([loadOverviewChart(), loadAlerts(), loadCatalog()]);
    const [debris, clusters] = await Promise.all([
      fetchAllDebrisForChart(),
      fetchAllClustersForChart(),
    ]);
    setChartExperimentDebris(debris);
    setChartExperimentClusters(clusters);
  }, [loadOverviewChart, loadAlerts, loadCatalog]);

  useEffect(() => {
    loadInitial();
  }, []);

  const handleUpdateData = async () => {
    setUpdating(true);
    try {
      await runIngestion();
      await runClustering(clusteringParams.current);
      await Promise.all([
        loadOverviewChart(),
        loadExperimentChart(),
        loadAlerts(),
        loadCatalog(),
      ]);
    } finally {
      setUpdating(false);
    }
  };

  const handleClusteringComplete = useCallback(
    async (result: ClusteringResultDto) => {
      clusteringParams.current = {
        epsilon: result.epsilon,
        minPoints: result.minPoints,
      };
      await Promise.all([loadExperimentChart(), loadAlerts(), loadCatalog()]);
    },
    [loadExperimentChart, loadAlerts, loadCatalog],
  );

  const onSegmentChange = (value: Segment) => {
    setSegment(value);
    if (value === 'clusters' && clusterPagination.items.length === 0) {
      clusterPagination.loadFirst();
    }
    if (value === 'debris' && debrisPagination.items.length === 0) {
      debrisPagination.loadFirst();
    }
  };

  const listHeader = (
    <View>
      <DashboardHeader
        debrisCount={debrisPagination.totalCount || chartOverviewDebris.length}
        clusterCount={clusterPagination.totalCount || chartOverviewClusters.length}
        alertCount={alertCount}
        chartDebris={chartOverviewDebris}
        chartClusters={chartOverviewClusters}
        onRefresh={handleUpdateData}
        refreshing={updating}
      />

      <ScatterChart
        debris={chartOverviewDebris}
        clusters={chartOverviewClusters}
        title="Visão geral do catálogo"
        subtitle="Estado atual · atualiza na ingestão e no pipeline completo"
      />

      <ClusteringPanel
        debris={chartExperimentDebris}
        clusters={chartExperimentClusters}
        onRunComplete={handleClusteringComplete}
        onParamsChange={params => {
          clusteringParams.current = params;
        }}
      />

      <SegmentTabs
        options={[
          { key: 'clusters', label: 'Clusters' },
          { key: 'debris', label: 'Detritos' },
        ]}
        value={segment}
        onChange={onSegmentChange}
      />
      <Text style={styles.sectionLabel}>
        CATÁLOGO · {segment === 'clusters' ? 'CLUSTERS DBSCAN' : 'DETRITOS TLE'}
      </Text>
      <Text style={styles.counter}>
        Registros {activePagination.items.length}/{activePagination.totalCount} · página{' '}
        {activePagination.page} de {activePagination.totalPages || 1}
      </Text>
      {activePagination.error ? (
        <Text style={styles.error}>{activePagination.error}</Text>
      ) : null}
    </View>
  );

  const listData: (ClusterDto | DebrisDto)[] =
    segment === 'clusters' ? clusterPagination.items : debrisPagination.items;

  return (
    <SafeAreaView style={styles.safe} edges={['left', 'right']}>
      <FlatList<ClusterDto | DebrisDto>
        style={isDesktop ? [styles.listDesktop, { maxWidth: contentMaxWidth }] : undefined}
        data={listData}
        key={segment}
        keyExtractor={item => item.id}
        ListHeaderComponent={listHeader}
        renderItem={({ item }) =>
          segment === 'clusters' ? (
            <ClusterRow cluster={item as ClusterDto} />
          ) : (
            <DebrisRow debris={item as DebrisDto} />
          )
        }
        contentContainerStyle={styles.list}
        onEndReached={activePagination.loadMore}
        onEndReachedThreshold={0.3}
        refreshControl={
          <RefreshControl
            refreshing={activePagination.refreshing}
            onRefresh={activePagination.refresh}
            tintColor={colors.primary}
          />
        }
        ListFooterComponent={
          activePagination.loading ? (
            <ActivityIndicator color={colors.primary} style={styles.footer} />
          ) : null
        }
      />
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  safe: {
    flex: 1,
    backgroundColor: colors.background,
    alignItems: 'center',
  },
  listDesktop: {
    width: '100%',
    alignSelf: 'center',
  },
  list: {
    padding: spacing.lg,
    paddingBottom: spacing.xl,
  },
  sectionLabel: {
    fontFamily: fontFamily.semiBold,
    fontSize: 11,
    color: colors.accentCyan,
    letterSpacing: 1,
    marginBottom: spacing.xs,
  },
  counter: {
    fontFamily: fontFamily.regular,
    color: colors.textMuted,
    fontSize: 12,
    marginBottom: spacing.sm,
  },
  error: {
    color: colors.danger,
    marginBottom: spacing.sm,
  },
  footer: {
    marginVertical: spacing.md,
  },
});
