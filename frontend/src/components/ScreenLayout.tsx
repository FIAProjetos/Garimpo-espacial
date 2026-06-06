import React from 'react';
import { ScrollView, StyleSheet, Text, type ViewStyle } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { ContentContainer } from './ContentContainer';
import { colors } from '../theme/colors';
import { spacing } from '../theme/spacing';

type Props = {
  title?: string;
  subtitle?: string;
  children: React.ReactNode;
  scroll?: boolean;
  narrow?: boolean;
  style?: ViewStyle;
};

export function ScreenLayout({
  title,
  subtitle,
  children,
  scroll = true,
  narrow = false,
  style,
}: Props) {
  const content = (
    <ContentContainer narrow={narrow} style={StyleSheet.flatten([styles.content, style])}>
      {title ? <Text style={styles.title}>{title}</Text> : null}
      {subtitle ? <Text style={styles.subtitle}>{subtitle}</Text> : null}
      {children}
    </ContentContainer>
  );

  return (
    <SafeAreaView style={styles.safe} edges={['top', 'left', 'right']}>
      {scroll ? (
        <ScrollView contentContainerStyle={styles.scrollContent}>{content}</ScrollView>
      ) : (
        content
      )}
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  safe: {
    flex: 1,
    backgroundColor: colors.background,
  },
  scrollContent: {
    flexGrow: 1,
    paddingHorizontal: spacing.lg,
    paddingBottom: spacing.xl,
  },
  content: {
    flex: 1,
    paddingTop: spacing.lg,
  },
  title: {
    fontSize: 28,
    fontWeight: '700',
    color: colors.text,
    marginBottom: spacing.sm,
  },
  subtitle: {
    fontSize: 16,
    color: colors.textMuted,
    marginBottom: spacing.lg,
    lineHeight: 22,
  },
});
