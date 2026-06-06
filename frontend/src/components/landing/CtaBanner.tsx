import React from 'react';
import { StyleSheet, Text, View } from 'react-native';
import { useNavigation } from '@react-navigation/native';
import type { NativeStackNavigationProp } from '@react-navigation/native-stack';
import { LinearGradient } from 'expo-linear-gradient';
import { useAuth } from '../../hooks/useAuth';
import { useResponsive } from '../../hooks/useResponsive';
import { colors } from '../../theme/colors';
import { spacing } from '../../theme/spacing';
import { typography } from '../../theme/typography';
import type { PublicStackParamList } from '../../navigation/types';
import { Button } from '../Button';
import { ContentContainer } from '../ContentContainer';

type Nav = NativeStackNavigationProp<PublicStackParamList>;

export function CtaBanner() {
  const navigation = useNavigation<Nav>();
  const { openRegisterModal } = useAuth();
  const { isDesktop } = useResponsive();

  return (
    <View style={styles.section}>
      <ContentContainer narrow>
        <LinearGradient
          colors={['rgba(59,130,246,0.15)', 'rgba(0,200,255,0.08)', colors.surface]}
          start={{ x: 0, y: 0 }}
          end={{ x: 1, y: 1 }}
          style={styles.banner}>
          <Text style={styles.title}>Pronto para garimpar o espaço?</Text>
          <Text style={styles.subtitle}>
            Crie sua conta gratuita e acesse o painel de análise de clusters, gráficos e alertas
            orbitais.
          </Text>
          <View style={[styles.ctas, isDesktop && styles.ctasDesktop]}>
            <Button label="Criar conta grátis" onPress={openRegisterModal} />
            <Button
              label="Ver planos"
              onPress={() => navigation.navigate('Pricing')}
              variant="pill"
            />
          </View>
        </LinearGradient>
      </ContentContainer>
    </View>
  );
}

const styles = StyleSheet.create({
  section: {
    paddingVertical: spacing.xl,
    paddingHorizontal: spacing.lg,
  },
  banner: {
    borderRadius: 20,
    borderWidth: 1,
    borderColor: colors.glow,
    padding: spacing.xl,
    alignItems: 'center',
  },
  title: {
    ...typography.h2,
    textAlign: 'center',
    marginBottom: spacing.sm,
  },
  subtitle: {
    ...typography.body,
    textAlign: 'center',
    maxWidth: 520,
    marginBottom: spacing.lg,
    color: colors.text,
    opacity: 0.85,
  },
  ctas: {
    gap: spacing.sm,
    width: '100%',
    maxWidth: 360,
  },
  ctasDesktop: {
    flexDirection: 'row',
    maxWidth: 480,
  },
});
