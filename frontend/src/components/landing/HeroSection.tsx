import React from 'react';
import { StyleSheet, Text, useWindowDimensions, View } from 'react-native';
import { useNavigation } from '@react-navigation/native';
import type { NativeStackNavigationProp } from '@react-navigation/native-stack';
import { heroSlides } from '../../assets/landingImages';
import { useAuth } from '../../hooks/useAuth';
import { useResponsive } from '../../hooks/useResponsive';
import { colors } from '../../theme/colors';
import { spacing } from '../../theme/spacing';
import { typography } from '../../theme/typography';
import type { PublicStackParamList } from '../../navigation/types';
import { Button } from '../Button';
import { HeroBackground } from '../HeroBackground';

type Nav = NativeStackNavigationProp<PublicStackParamList>;

export function HeroSection() {
  const { height: windowHeight } = useWindowDimensions();
  const { isDesktop } = useResponsive();
  const navigation = useNavigation<Nav>();
  const { openRegisterModal } = useAuth();

  const heroHeight = Math.max(windowHeight * 0.88, 520);

  return (
    <View style={[styles.wrap, { minHeight: heroHeight }]}>
      <HeroBackground images={heroSlides} height={heroHeight} />
      <View style={[styles.content, isDesktop ? styles.contentDesktop : styles.contentMobile]}>
        <Text style={[styles.tag, !isDesktop && styles.centeredText]}>
          Global Solution · Exploração Espacial
        </Text>
        <Text style={[typography.display, styles.headline, !isDesktop && styles.centeredText]}>
          Inteligência orbital para um céu mais seguro
        </Text>
        <Text style={[typography.body, styles.lead, !isDesktop && styles.centeredText]}>
          Transformamos telemetria TLE pública em mapas de densidade, clusters de risco e
          alertas acionáveis para equipes de análise orbital.
        </Text>
        <View style={[styles.ctas, isDesktop && styles.ctasDesktop]}>
          <Button
            label="Criar conta grátis"
            onPress={openRegisterModal}
            style={isDesktop ? styles.ctaBtn : undefined}
          />
          <Button
            label="Ver planos"
            onPress={() => navigation.navigate('Pricing')}
            variant="pill"
            style={isDesktop ? styles.ctaBtn : undefined}
          />
        </View>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  wrap: {
    position: 'relative',
    justifyContent: 'flex-end',
  },
  content: {
    position: 'absolute',
    bottom: 0,
    left: 0,
    right: 0,
    paddingHorizontal: spacing.lg,
    paddingBottom: spacing.xl,
  },
  contentDesktop: {
    maxWidth: 1080,
    alignSelf: 'center',
    width: '100%',
    paddingBottom: spacing.xl * 1.5,
    paddingLeft: spacing.xl,
  },
  contentMobile: {
    alignItems: 'center',
  },
  tag: {
    ...typography.label,
    color: colors.accentCyan,
    marginBottom: spacing.sm,
  },
  headline: {
    maxWidth: 640,
    marginBottom: spacing.md,
  },
  lead: {
    maxWidth: 520,
    marginBottom: spacing.lg,
    color: colors.text,
    opacity: 0.9,
  },
  ctas: {
    gap: spacing.sm,
    width: '100%',
    maxWidth: 400,
  },
  ctasDesktop: {
    flexDirection: 'row',
    maxWidth: 480,
  },
  ctaBtn: {
    flex: 1,
    minWidth: 160,
  },
  centeredText: {
    textAlign: 'center',
  },
});
