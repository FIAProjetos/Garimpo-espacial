import React from 'react';
import { ScrollView, StyleSheet } from 'react-native';
import { CtaBanner } from '../components/landing/CtaBanner';
import { FeaturesSection } from '../components/landing/FeaturesSection';
import { HeroSection } from '../components/landing/HeroSection';
import { HowItWorksSection } from '../components/landing/HowItWorksSection';
import { KesslerSection } from '../components/landing/KesslerSection';
import { PublicFooter } from '../components/landing/PublicFooter';
import { PublicLayout } from '../components/PublicLayout';
import { colors } from '../theme/colors';

export function HomeScreen() {
  return (
    <PublicLayout>
      <ScrollView
        style={styles.scroll}
        contentContainerStyle={styles.content}
        showsVerticalScrollIndicator={false}>
        <HeroSection />
        <KesslerSection />
        <HowItWorksSection />
        <FeaturesSection />
        <CtaBanner />
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
});
