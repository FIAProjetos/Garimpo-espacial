import React from 'react';
import { StyleSheet, Text, View } from 'react-native';
import { useResponsive } from '../../hooks/useResponsive';
import { colors } from '../../theme/colors';
import { spacing } from '../../theme/spacing';
import { typography } from '../../theme/typography';
import { ContentContainer } from '../ContentContainer';
import { SectionHeader } from './SectionHeader';

const steps = [
  {
    step: '01',
    title: 'Ingestão TLE',
    description:
      'Coletamos elementos de duas linhas do catálogo orbital público e normalizamos altitude, inclinação e metadados.',
  },
  {
    step: '02',
    title: 'Clustering DBSCAN',
    description:
      'Agrupamos detritos em zonas de alta densidade para identificar aglomerados de risco em LEO.',
  },
  {
    step: '03',
    title: 'Alertas acionáveis',
    description:
      'Geramos notificações de densidade e integridade para apoiar decisões de missão e mitigação.',
  },
];

export function HowItWorksSection() {
  const { isDesktop } = useResponsive();

  return (
    <View style={styles.section}>
      <ContentContainer>
        <SectionHeader
          label="Como funciona"
          title="Da telemetria ao insight orbital"
          subtitle="Três etapas para transformar dados brutos em inteligência de missão."
          centered
        />
        <View style={[styles.grid, isDesktop && styles.gridDesktop]}>
          {steps.map(item => (
            <View key={item.step} style={styles.card}>
              <Text style={styles.step}>{item.step}</Text>
              <Text style={styles.title}>{item.title}</Text>
              <Text style={styles.description}>{item.description}</Text>
            </View>
          ))}
        </View>
      </ContentContainer>
    </View>
  );
}

const styles = StyleSheet.create({
  section: {
    paddingVertical: spacing.xl * 1.5,
    backgroundColor: colors.surface,
    borderTopWidth: 1,
    borderBottomWidth: 1,
    borderColor: colors.surfaceBorder,
  },
  grid: {
    gap: spacing.md,
  },
  gridDesktop: {
    flexDirection: 'row',
  },
  card: {
    flex: 1,
    backgroundColor: colors.background,
    borderRadius: 16,
    borderWidth: 1,
    borderColor: colors.surfaceBorder,
    padding: spacing.lg,
  },
  step: {
    ...typography.label,
    color: colors.primary,
    marginBottom: spacing.sm,
  },
  title: {
    ...typography.h3,
    marginBottom: spacing.sm,
  },
  description: {
    ...typography.body,
    lineHeight: 22,
  },
});
