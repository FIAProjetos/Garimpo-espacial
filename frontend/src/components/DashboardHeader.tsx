import React, { useMemo } from 'react';
import { StyleSheet, Text, View } from 'react-native';
import { useResponsive } from '../hooks/useResponsive';
import { analyst } from '../theme/analyst';
import { colors } from '../theme/colors';
import { spacing } from '../theme/spacing';
import { fontFamily, typography } from '../theme/typography';
import type { ClusterDto, DebrisDto } from '../types/api';
import { Button } from './Button';
import { DataField } from './DataField';

type Props = {
  debrisCount: number;
  clusterCount: number;
  alertCount: number;
  chartDebris: DebrisDto[];
  chartClusters: ClusterDto[];
  onRefresh: () => void;
  refreshing: boolean;
};

export function DashboardHeader({
  debrisCount,
  clusterCount,
  alertCount,
  chartDebris,
  chartClusters,
  onRefresh,
  refreshing,
}: Props) {
  const { isDesktop } = useResponsive();

  const metrics = useMemo(() => {
    const avgAlt =
      chartDebris.length > 0
        ? chartDebris.reduce((s, d) => s + d.altitudeKm, 0) / chartDebris.length
        : 0;
    const maxDensity =
      chartClusters.length > 0 ? Math.max(...chartClusters.map(c => c.density)) : 0;
    const clusteredPct =
      chartDebris.length > 0
        ? (chartDebris.filter(d => d.clusterId).length / chartDebris.length) * 100
        : 0;

    return { avgAlt, maxDensity, clusteredPct };
  }, [chartDebris, chartClusters]);

  return (
    <View style={[styles.container, isDesktop && styles.containerDesktop]}>
      <View style={styles.titleRow}>
        <View>
          <Text style={styles.tag}>MISSION CONTROL</Text>
          <Text style={styles.heading}>Painel de Análise Orbital</Text>
          <Text style={styles.subtitle}>LEO · DBSCAN · TLE público · Celestrak</Text>
        </View>
        <View style={[styles.alertBadge, alertCount > 0 && styles.alertBadgeActive]}>
          <Text style={styles.alertBadgeLabel}>ALERTAS</Text>
          <Text style={[styles.alertBadgeValue, alertCount > 0 && styles.alertBadgeValueActive]}>
            {alertCount}
          </Text>
        </View>
      </View>

      <View style={styles.statsGrid}>
        <StatPanel label="Detritos catalogados" value={String(debrisCount)} unit="obj" />
        <StatPanel label="Clusters DBSCAN" value={String(clusterCount)} unit="grp" />
        <StatPanel
          label="Densidade máx."
          value={metrics.maxDensity.toFixed(2)}
          unit="ρ"
          highlight
        />
        <StatPanel label="Altitude média" value={metrics.avgAlt.toFixed(0)} unit="km" />
        <StatPanel label="Taxa clusterizada" value={metrics.clusteredPct.toFixed(1)} unit="%" />
      </View>

      <View style={styles.dataRow}>
        <DataField label="Pipeline" value={refreshing ? 'SYNC' : 'READY'} highlight={!refreshing} />
        <DataField label="Algoritmo" value="DBSCAN" />
        <DataField label="Fonte" value="TLE" />
        <DataField label="Órbita" value="LEO" />
      </View>

      <Button
        label={refreshing ? 'Ingestão + pipeline...' : 'Ingestão + pipeline completo'}
        onPress={onRefresh}
        loading={refreshing}
        style={isDesktop ? styles.refreshDesktop : undefined}
      />
    </View>
  );
}

function StatPanel({
  label,
  value,
  unit,
  highlight,
}: {
  label: string;
  value: string;
  unit?: string;
  highlight?: boolean;
}) {
  return (
    <View style={[styles.stat, highlight && styles.statHighlight]}>
      <Text style={styles.statLabel}>{label}</Text>
      <Text style={[styles.statValue, highlight && styles.statValueHighlight]}>
        {value}
        {unit ? <Text style={styles.statUnit}> {unit}</Text> : null}
      </Text>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    marginBottom: spacing.md,
  },
  containerDesktop: {
    flex: 1,
    marginBottom: 0,
  },
  titleRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
    marginBottom: spacing.md,
  },
  tag: {
    ...typography.label,
    color: colors.accentCyan,
    marginBottom: spacing.xs,
  },
  heading: {
    fontFamily: fontFamily.bold,
    fontSize: 20,
    color: colors.text,
  },
  subtitle: {
    fontFamily: fontFamily.regular,
    fontSize: 12,
    color: colors.textMuted,
    marginTop: spacing.xs,
    letterSpacing: 0.3,
  },
  alertBadge: {
    backgroundColor: analyst.panelBg,
    borderWidth: 1,
    borderColor: analyst.panelBorder,
    borderRadius: 8,
    paddingHorizontal: spacing.md,
    paddingVertical: spacing.sm,
    alignItems: 'center',
    minWidth: 72,
  },
  alertBadgeActive: {
    borderColor: colors.danger,
    backgroundColor: 'rgba(239, 68, 68, 0.1)',
  },
  alertBadgeLabel: {
    fontFamily: fontFamily.regular,
    fontSize: 9,
    color: colors.textMuted,
    letterSpacing: 1,
  },
  alertBadgeValue: {
    fontFamily: fontFamily.bold,
    fontSize: 22,
    color: colors.textMuted,
  },
  alertBadgeValueActive: {
    color: colors.danger,
  },
  statsGrid: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: spacing.sm,
    marginBottom: spacing.md,
  },
  stat: {
    flexGrow: 1,
    minWidth: 100,
    backgroundColor: analyst.panelBg,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: analyst.panelBorder,
    padding: spacing.sm,
    paddingHorizontal: spacing.md,
  },
  statHighlight: {
    borderColor: 'rgba(0, 200, 255, 0.35)',
  },
  statLabel: {
    fontFamily: fontFamily.regular,
    fontSize: 10,
    color: colors.textMuted,
    textTransform: 'uppercase',
    letterSpacing: 0.5,
    marginBottom: 4,
  },
  statValue: {
    fontFamily: fontFamily.bold,
    fontSize: 18,
    color: colors.primary,
  },
  statValueHighlight: {
    color: colors.accentCyan,
  },
  statUnit: {
    fontFamily: fontFamily.regular,
    fontSize: 11,
    color: colors.textMuted,
  },
  dataRow: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: spacing.lg,
    marginBottom: spacing.md,
    paddingVertical: spacing.sm,
    borderTopWidth: 1,
    borderBottomWidth: 1,
    borderColor: analyst.panelBorder,
  },
  refreshDesktop: {
    alignSelf: 'flex-start',
    minWidth: 200,
  },
});
