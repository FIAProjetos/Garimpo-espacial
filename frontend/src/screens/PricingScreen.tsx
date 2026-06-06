import React from 'react';
import { ScrollView, StyleSheet, View } from 'react-native';
import { ContentContainer } from '../components/ContentContainer';
import { PlanCard } from '../components/PlanCard';
import { PublicLayout } from '../components/PublicLayout';
import { PublicFooter } from '../components/landing/PublicFooter';
import { SectionHeader } from '../components/landing/SectionHeader';
import { useAuth } from '../hooks/useAuth';
import { useResponsive } from '../hooks/useResponsive';
import { colors } from '../theme/colors';
import { spacing } from '../theme/spacing';

const NAVBAR_OFFSET = 72;

export function PricingScreen() {
  const { isDesktop } = useResponsive();
  const { openRegisterModal } = useAuth();

  return (
    <PublicLayout>
      <ScrollView
        style={styles.scroll}
        contentContainerStyle={styles.content}
        showsVerticalScrollIndicator={false}>
        <View style={styles.page}>
          <ContentContainer style={styles.headerWrap}>
            <SectionHeader
              label="Planos"
              title="Escolha como explorar o espaço"
              subtitle="Comece gratuitamente e explore o catálogo orbital. O plano Pro chega em breve."
              centered
            />
            <View style={[styles.row, isDesktop ? styles.rowDesktop : styles.rowMobile]}>
              <PlanCard
                name="Beta Gratuito"
                price="R$ 0"
                features={[
                  'Painel de análise de clusters',
                  'Gráficos altitude × inclinação',
                  'Alertas de densidade e integridade',
                ]}
                ctaLabel="Criar conta grátis"
                onPress={openRegisterModal}
              />
              <PlanCard
                name="Garimpo Pro"
                price="R$ 29/mês"
                features={[
                  'API avançada e exportação',
                  'Alertas em tempo real',
                  'Suporte prioritário',
                ]}
                ctaLabel="Indisponível"
                wip
              />
            </View>
          </ContentContainer>
        </View>
        <PublicFooter />
      </ScrollView>
    </PublicLayout>
  );
}

const styles = StyleSheet.create({
  scroll: {
    flex: 1,
    backgroundColor: colors.background,
  },
  content: {
    flexGrow: 1,
  },
  page: {
    paddingTop: NAVBAR_OFFSET + spacing.lg,
    paddingBottom: spacing.xl,
    paddingHorizontal: spacing.lg,
  },
  headerWrap: {
    alignItems: 'center',
  },
  row: {
    gap: spacing.md,
    width: '100%',
  },
  rowDesktop: {
    flexDirection: 'row',
    maxWidth: 820,
    justifyContent: 'center',
    alignSelf: 'center',
  },
  rowMobile: {
    flexDirection: 'column',
  },
});
