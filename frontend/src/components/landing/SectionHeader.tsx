import React from 'react';
import { StyleSheet, Text, View } from 'react-native';
import { spacing } from '../../theme/spacing';
import { typography } from '../../theme/typography';

type Props = {
  label?: string;
  title: string;
  subtitle?: string;
  centered?: boolean;
};

export function SectionHeader({ label, title, subtitle, centered = false }: Props) {
  return (
    <View style={[styles.wrap, centered && styles.centered]}>
      {label ? <Text style={[typography.label, styles.label]}>{label}</Text> : null}
      <Text style={[typography.h1, centered && styles.centeredText]}>{title}</Text>
      {subtitle ? (
        <Text style={[typography.body, styles.subtitle, centered && styles.centeredText]}>
          {subtitle}
        </Text>
      ) : null}
    </View>
  );
}

const styles = StyleSheet.create({
  wrap: {
    marginBottom: spacing.xl,
  },
  centered: {
    alignItems: 'center',
  },
  label: {
    marginBottom: spacing.sm,
  },
  subtitle: {
    marginTop: spacing.sm,
    maxWidth: 640,
  },
  centeredText: {
    textAlign: 'center',
  },
});
