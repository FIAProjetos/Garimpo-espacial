import React, { useCallback, useEffect, useMemo, useState } from 'react';
import {
  ActivityIndicator,
  FlatList,
  RefreshControl,
  StyleSheet,
  Text,
  View,
} from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { DataField } from '../components/DataField';
import { getAlerts } from '../services/api';
import { useResponsive } from '../hooks/useResponsive';
import { analyst } from '../theme/analyst';
import { colors } from '../theme/colors';
import { spacing } from '../theme/spacing';
import { fontFamily, typography } from '../theme/typography';
import type { AlertDto } from '../types/api';

export function AlertsScreen() {
  const { isDesktop, contentMaxWidth } = useResponsive();
  const [alerts, setAlerts] = useState<AlertDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);

  const load = useCallback(async () => {
    const data = await getAlerts();
    setAlerts(data);
  }, []);

  useEffect(() => {
    load().finally(() => setLoading(false));
  }, [load]);

  const refresh = async () => {
    setRefreshing(true);
    await load();
    setRefreshing(false);
  };

  const summary = useMemo(() => {
    const critical = alerts.filter(a => a.severity === 'Critical').length;
    const active = alerts.filter(a => !a.isAcknowledged).length;
    const immediate = alerts.filter(a => a.requiresImmediateAction).length;
    return { total: alerts.length, critical, active, immediate };
  }, [alerts]);

  if (loading) {
    return (
      <SafeAreaView style={styles.safe}>
        <ActivityIndicator color={colors.primary} style={styles.loader} />
      </SafeAreaView>
    );
  }

  return (
    <SafeAreaView style={styles.safe} edges={['left', 'right', 'bottom']}>
      <View style={[styles.inner, isDesktop && { maxWidth: contentMaxWidth }]}>
        <Text style={styles.tag}>MONITORAMENTO ORBITAL</Text>
        <Text style={styles.title}>Central de Alertas</Text>
        <Text style={styles.subtitle}>
          Eventos de densidade, integridade e risco em LEO — classificados por severidade.
        </Text>

        <View style={styles.summary}>
          <SummaryCard label="Total" value={summary.total} />
          <SummaryCard label="Ativos" value={summary.active} highlight={summary.active > 0} />
          <SummaryCard label="Críticos" value={summary.critical} danger={summary.critical > 0} />
          <SummaryCard label="Ação imediata" value={summary.immediate} warn={summary.immediate > 0} />
        </View>

        <FlatList
          data={alerts}
          keyExtractor={item => item.id}
          contentContainerStyle={styles.list}
          refreshControl={
            <RefreshControl
              refreshing={refreshing}
              onRefresh={refresh}
              tintColor={colors.primary}
            />
          }
          ListEmptyComponent={
            <Text style={styles.empty}>Nenhum alerta registrado. Sistema nominal.</Text>
          }
          renderItem={({ item }) => (
            <View
              style={[
                styles.card,
                item.severity === 'Critical' && styles.cardCritical,
                !item.isAcknowledged && styles.cardActive,
              ]}>
              <View style={styles.row}>
                <Text style={styles.type}>{item.alertType}</Text>
                <View
                  style={[
                    styles.severityBadge,
                    item.severity === 'Critical' && styles.severityCritical,
                  ]}>
                  <Text style={styles.severity}>{item.severity.toUpperCase()}</Text>
                </View>
              </View>
              <Text style={styles.message}>{item.message}</Text>
              <View style={styles.metaGrid}>
                <DataField
                  label="Status"
                  value={item.isAcknowledged ? 'ACK' : 'ATIVO'}
                  highlight={!item.isAcknowledged}
                />
                <DataField
                  label="Ação"
                  value={item.requiresImmediateAction ? 'SIM' : 'NÃO'}
                />
                <DataField
                  label="Disparo"
                  value={new Date(item.triggeredAt).toLocaleString('pt-BR')}
                />
              </View>
            </View>
          )}
        />
      </View>
    </SafeAreaView>
  );
}

function SummaryCard({
  label,
  value,
  highlight,
  danger,
  warn,
}: {
  label: string;
  value: number;
  highlight?: boolean;
  danger?: boolean;
  warn?: boolean;
}) {
  return (
    <View
      style={[
        styles.summaryCard,
        highlight && styles.summaryHighlight,
        danger && styles.summaryDanger,
        warn && styles.summaryWarn,
      ]}>
      <Text style={styles.summaryLabel}>{label}</Text>
      <Text style={styles.summaryValue}>{value}</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  safe: {
    flex: 1,
    backgroundColor: colors.background,
    alignItems: 'center',
  },
  inner: {
    flex: 1,
    width: '100%',
    paddingHorizontal: spacing.lg,
    paddingTop: spacing.md,
  },
  tag: {
    ...typography.label,
    color: colors.accentCyan,
    marginBottom: spacing.xs,
  },
  title: {
    fontFamily: fontFamily.bold,
    fontSize: 22,
    color: colors.text,
  },
  subtitle: {
    ...typography.body,
    marginBottom: spacing.md,
    lineHeight: 22,
  },
  summary: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: spacing.sm,
    marginBottom: spacing.lg,
  },
  summaryCard: {
    flexGrow: 1,
    minWidth: 72,
    backgroundColor: analyst.panelBg,
    borderWidth: 1,
    borderColor: analyst.panelBorder,
    borderRadius: 8,
    padding: spacing.sm,
    alignItems: 'center',
  },
  summaryHighlight: {
    borderColor: colors.primary,
  },
  summaryDanger: {
    borderColor: colors.danger,
    backgroundColor: 'rgba(239, 68, 68, 0.08)',
  },
  summaryWarn: {
    borderColor: colors.secondary,
  },
  summaryLabel: {
    fontFamily: fontFamily.regular,
    fontSize: 9,
    color: colors.textMuted,
    textTransform: 'uppercase',
    letterSpacing: 0.5,
  },
  summaryValue: {
    fontFamily: fontFamily.bold,
    fontSize: 20,
    color: colors.text,
  },
  list: {
    paddingBottom: spacing.xl,
  },
  loader: {
    marginTop: spacing.xl,
  },
  card: {
    backgroundColor: analyst.panelBg,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: analyst.panelBorder,
    padding: spacing.md,
    marginBottom: spacing.sm,
  },
  cardCritical: {
    borderLeftWidth: 3,
    borderLeftColor: colors.danger,
  },
  cardActive: {
    borderColor: 'rgba(59, 130, 246, 0.35)',
  },
  row: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: spacing.xs,
  },
  type: {
    fontFamily: fontFamily.semiBold,
    fontSize: 14,
    color: colors.text,
  },
  severityBadge: {
    backgroundColor: 'rgba(245, 158, 11, 0.15)',
    borderWidth: 1,
    borderColor: colors.secondary,
    paddingHorizontal: spacing.sm,
    paddingVertical: 2,
    borderRadius: 4,
  },
  severityCritical: {
    backgroundColor: 'rgba(239, 68, 68, 0.15)',
    borderColor: colors.danger,
  },
  severity: {
    fontFamily: fontFamily.medium,
    fontSize: 10,
    color: colors.secondary,
    letterSpacing: 0.5,
  },
  message: {
    fontFamily: fontFamily.regular,
    color: colors.textMuted,
    lineHeight: 20,
    marginBottom: spacing.sm,
    fontSize: 14,
  },
  metaGrid: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: spacing.md,
  },
  empty: {
    fontFamily: fontFamily.regular,
    color: colors.textMuted,
    textAlign: 'center',
    marginTop: spacing.xl,
  },
});
