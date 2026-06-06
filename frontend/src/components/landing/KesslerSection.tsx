import React from 'react';
import { Image, StyleSheet, Text, View } from 'react-native';
import { landingImages } from '../../assets/landingImages';
import { useResponsive } from '../../hooks/useResponsive';
import { colors } from '../../theme/colors';
import { spacing } from '../../theme/spacing';
import { fontFamily, typography } from '../../theme/typography';
import { ContentContainer } from '../ContentContainer';
import { SectionHeader } from './SectionHeader';

const stats = [
  {
    title: 'Efeito em cascata',
    description: 'Cada colisão gera mais fragmentos e aumenta o risco de novos impactos.',
  },
  {
    title: 'LEO em foco',
    description: 'A órbita baixa da Terra concentra satélites, estações e detritos em alta densidade.',
  },
  {
    title: 'Detecção antecipada',
    description: 'Clusters DBSCAN revelam zonas críticas antes que uma colisão se torne inevitável.',
  },
];

export function KesslerSection() {
  const { isDesktop } = useResponsive();

  return (
    <View style={styles.section}>
      <ContentContainer>
        <SectionHeader
          label="O problema orbital"
          title="A Síndrome de Kessler"
          subtitle="Por que monitorar detritos espaciais deixou de ser opcional."
        />
        <View style={[styles.body, isDesktop && styles.bodyDesktop]}>
          <Image
            source={landingImages.debris}
            style={[styles.image, isDesktop && styles.imageDesktop]}
            resizeMode="cover"
          />
          <View style={styles.textCol}>
            <Text style={styles.paragraph}>
              A <Text style={styles.strong}>Síndrome de Kessler</Text> descreve um cenário em que
              colisões entre satélites e detritos geram ainda mais fragmentos, aumentando a
              probabilidade de novos impactos — um efeito em cascata na órbita baixa da Terra (LEO).
            </Text>
            <Text style={styles.paragraph}>
              Com milhares de objetos rastreados e milhões de fragmentos menores, monitorar{' '}
              <Text style={styles.strong}>zonas de alta densidade</Text> é essencial para proteger
              missões ativas e infraestrutura orbital.
            </Text>
            <Text style={styles.paragraph}>
              O <Text style={styles.strong}>Garimpo Espacial</Text> transforma dados TLE públicos em
              mapas de densidade, clusters de risco e alertas — ajudando equipes a enxergar o
              problema antes que uma colisão defina o próximo capítulo da história orbital.
            </Text>
          </View>
        </View>
        <View style={[styles.stats, isDesktop && styles.statsDesktop]}>
          {stats.map(stat => (
            <View key={stat.title} style={styles.statCard}>
              <Text style={styles.statTitle}>{stat.title}</Text>
              <Text style={styles.statDesc}>{stat.description}</Text>
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
  body: {
    gap: spacing.lg,
    marginBottom: spacing.xl,
  },
  bodyDesktop: {
    flexDirection: 'row',
    alignItems: 'flex-start',
  },
  image: {
    width: '100%',
    height: 220,
    borderRadius: 16,
    borderWidth: 1,
    borderColor: colors.glow,
  },
  imageDesktop: {
    flex: 1,
    height: 320,
    maxWidth: '48%',
  },
  textCol: {
    flex: 1,
    gap: spacing.md,
  },
  paragraph: {
    ...typography.body,
    color: colors.textMuted,
    lineHeight: 26,
  },
  strong: {
    fontFamily: fontFamily.semiBold,
    color: colors.text,
  },
  stats: {
    gap: spacing.md,
  },
  statsDesktop: {
    flexDirection: 'row',
  },
  statCard: {
    flex: 1,
    backgroundColor: colors.surface,
    borderRadius: 14,
    borderWidth: 1,
    borderColor: colors.surfaceBorder,
    padding: spacing.lg,
    borderLeftWidth: 3,
    borderLeftColor: colors.accentCyan,
  },
  statTitle: {
    ...typography.h3,
    marginBottom: spacing.xs,
  },
  statDesc: {
    ...typography.caption,
    lineHeight: 20,
  },
});
