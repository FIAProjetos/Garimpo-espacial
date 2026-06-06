import React from 'react';
import { StyleSheet, Text, View } from 'react-native';
import { DataField } from './DataField';
import { analyst } from '../theme/analyst';
import { colors } from '../theme/colors';
import { spacing } from '../theme/spacing';
import { fontFamily } from '../theme/typography';
import type { ClusterDto } from '../types/api';

type Props = {
  cluster: ClusterDto;
};

export function ClusterRow({ cluster }: Props) {
  const densityLevel =
    cluster.density > 0.7 ? 'ALTA' : cluster.density > 0.4 ? 'MÉDIA' : 'BAIXA';

  return (
    <View style={styles.card}>
      <View style={styles.header}>
        <Text style={styles.title}>Cluster #{cluster.label}</Text>
        <View style={[styles.badge, densityLevel === 'ALTA' && styles.badgeHigh]}>
          <Text style={styles.badgeText}>ρ {densityLevel}</Text>
        </View>
      </View>
      <View style={styles.grid}>
        <DataField label="Densidade" value={cluster.density.toFixed(3)} highlight />
        <DataField label="Membros" value={String(cluster.memberCount)} unit="obj" />
        <DataField label="Alt. centróide" value={cluster.centroidAltitudeKm.toFixed(0)} unit="km" />
        <DataField
          label="Inc. centróide"
          value={cluster.centroidInclinationDegrees.toFixed(1)}
          unit="°"
        />
      </View>
      <Text style={styles.meta}>
        ID {cluster.id.slice(0, 8)}… · {new Date(cluster.createdAt).toLocaleString('pt-BR')} UTC
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
    borderLeftWidth: 3,
    borderLeftColor: colors.accentCyan,
    padding: spacing.md,
    marginBottom: spacing.sm,
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: spacing.sm,
  },
  title: {
    fontFamily: fontFamily.semiBold,
    fontSize: 15,
    color: colors.text,
  },
  badge: {
    backgroundColor: 'rgba(245, 158, 11, 0.15)',
    borderWidth: 1,
    borderColor: colors.secondary,
    paddingHorizontal: spacing.sm,
    paddingVertical: 2,
    borderRadius: 4,
  },
  badgeHigh: {
    backgroundColor: 'rgba(239, 68, 68, 0.15)',
    borderColor: colors.danger,
  },
  badgeText: {
    fontFamily: fontFamily.medium,
    fontSize: 10,
    color: colors.secondary,
    letterSpacing: 0.5,
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
    letterSpacing: 0.3,
  },
});
