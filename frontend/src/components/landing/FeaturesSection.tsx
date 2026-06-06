import React from 'react';
import { StyleSheet, Text, View } from 'react-native';
import { useResponsive } from '../../hooks/useResponsive';
import { colors } from '../../theme/colors';
import { spacing } from '../../theme/spacing';
import { typography } from '../../theme/typography';
import { ContentContainer } from '../ContentContainer';
import { SectionHeader } from './SectionHeader';

const features = [
  {
    title: 'Mapas de densidade',
    description: 'Visualize a distribuição de detritos por altitude e inclinação em gráficos interativos.',
  },
  {
    title: 'Clusters de risco',
    description: 'Identifique aglomerados críticos com DBSCAN e acompanhe centroides orbitais.',
  },
  {
    title: 'Alertas em tempo real',
    description: 'Monitore eventos de densidade e integridade com severidade classificada.',
  },
  {
    title: 'Painel unificado',
    description: 'Dashboard com estatísticas, gráficos e listas paginadas de clusters e detritos.',
  },
  {
    title: 'Dados TLE públicos',
    description: 'Baseado em telemetria orbital aberta, sem dependência de fontes proprietárias.',
  },
  {
    title: 'Beta gratuito',
    description: 'Comece sem custo e explore o catálogo orbital com conta de analista.',
  },
];

export function FeaturesSection() {
  const { isDesktop } = useResponsive();

  return (
    <View style={styles.section}>
      <ContentContainer>
        <SectionHeader
          label="Recursos"
          title="Tudo que você precisa para analisar LEO"
          subtitle="Ferramentas pensadas para equipes de exploração espacial e mitigação de risco."
          centered
        />
        <View style={[styles.grid, isDesktop && styles.gridDesktop]}>
          {features.map(feature => (
            <View key={feature.title} style={[styles.card, isDesktop && styles.cardDesktop]}>
              <Text style={styles.title}>{feature.title}</Text>
              <Text style={styles.description}>{feature.description}</Text>
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
    backgroundColor: colors.background,
  },
  grid: {
    gap: spacing.md,
  },
  gridDesktop: {
    flexDirection: 'row',
    flexWrap: 'wrap',
  },
  card: {
    backgroundColor: colors.surface,
    borderRadius: 14,
    borderWidth: 1,
    borderColor: colors.surfaceBorder,
    padding: spacing.lg,
  },
  cardDesktop: {
    flexBasis: '31%',
    flexGrow: 1,
    minWidth: 280,
    maxWidth: '33%',
  },
  title: {
    ...typography.h3,
    marginBottom: spacing.xs,
  },
  description: {
    ...typography.body,
    lineHeight: 22,
  },
});
