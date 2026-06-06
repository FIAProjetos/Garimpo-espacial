import React from 'react';
import { StyleSheet, Text, View } from 'react-native';
import { DataField } from './DataField';
import { analyst } from '../theme/analyst';
import { colors } from '../theme/colors';
import { spacing } from '../theme/spacing';
import { fontFamily } from '../theme/typography';
import type { DebrisDto } from '../types/api';

type Props = {
  debris: DebrisDto;
};

export function DebrisRow({ debris }: Props) {
  return (
    <View style={styles.card}>
      <View style={styles.header}>
        <Text style={styles.title} numberOfLines={1}>
          {debris.name}
        </Text>
        <View style={[styles.badge, debris.clusterId && styles.badgeClustered]}>
          <Text style={styles.badgeText}>
            {debris.clusterId ? 'CLUSTER' : 'ISOLADO'}
          </Text>
        </View>
      </View>
      <View style={styles.grid}>
        <DataField label="NORAD ID" value={String(debris.noradId)} highlight />
        <DataField label="Altitude" value={debris.altitudeKm.toFixed(0)} unit="km" />
        <DataField label="Inclinação" value={debris.inclinationDegrees.toFixed(2)} unit="°" />
        <DataField label="Excentricidade" value={debris.eccentricity.toFixed(5)} />
        <DataField label="Mov. médio" value={debris.meanMotionRevsPerDay.toFixed(4)} unit="rev/d" />
        <DataField label="Classificação" value={debris.classification} />
      </View>
      <Text style={styles.meta}>
        Capturado {new Date(debris.capturedAt).toLocaleString('pt-BR')}
        {debris.clusterId ? ` · Cluster ${debris.clusterId.slice(0, 8)}…` : ''}
      </Text>
    </View>
  );
}

const styles = StyleSheet.create({
  card: {
    backgroundColor: analyst.panelBg,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: analyst.panelBorder,
    padding: spacing.md,
    marginBottom: spacing.sm,
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: spacing.sm,
    gap: spacing.sm,
  },
  title: {
    fontFamily: fontFamily.semiBold,
    fontSize: 14,
    color: colors.text,
    flex: 1,
  },
  badge: {
    backgroundColor: colors.surfaceBorder,
    paddingHorizontal: spacing.sm,
    paddingVertical: 2,
    borderRadius: 4,
  },
  badgeClustered: {
    backgroundColor: 'rgba(59, 130, 246, 0.15)',
    borderWidth: 1,
    borderColor: colors.primary,
  },
  badgeText: {
    fontFamily: fontFamily.medium,
    fontSize: 9,
    color: colors.textMuted,
    letterSpacing: 0.8,
  },
  grid: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: spacing.md,
    marginBottom: spacing.sm,
  },
  meta: {
    fontFamily: fontFamily.regular,
    fontSize: 10,
    color: colors.textMuted,
  },
});
