import React from 'react';
import { StyleSheet, Text, View } from 'react-native';
import { colors } from '../theme/colors';
import { spacing } from '../theme/spacing';
import { fontFamily } from '../theme/typography';
import { Button } from './Button';

type Props = {
  name: string;
  price: string;
  features: string[];
  ctaLabel: string;
  onPress?: () => void;
  wip?: boolean;
};

export function PlanCard({
  name,
  price,
  features,
  ctaLabel,
  onPress,
  wip = false,
}: Props) {
  return (
    <View style={[styles.card, wip && styles.cardWip]}>
      {wip ? (
        <View style={styles.badge}>
          <Text style={styles.badgeText}>Em breve</Text>
        </View>
      ) : null}
      <Text style={styles.name}>{name}</Text>
      <Text style={styles.price}>{price}</Text>
      {features.map(feature => (
        <Text key={feature} style={styles.feature}>
          • {feature}
        </Text>
      ))}
      <Button
        label={ctaLabel}
        onPress={onPress ?? (() => {})}
        variant={wip ? 'disabled' : 'primary'}
        style={styles.cta}
      />
    </View>
  );
}

const styles = StyleSheet.create({
  card: {
    flex: 1,
    backgroundColor: colors.surface,
    borderRadius: 16,
    borderWidth: 1,
    borderColor: colors.surfaceBorder,
    padding: spacing.lg,
    minWidth: 150,
  },
  cardWip: {
    opacity: 0.55,
  },
  badge: {
    alignSelf: 'flex-start',
    backgroundColor: colors.wip,
    paddingHorizontal: spacing.sm,
    paddingVertical: spacing.xs,
    borderRadius: 8,
    marginBottom: spacing.sm,
  },
  badgeText: {
    color: colors.text,
    fontSize: 12,
    fontWeight: '600',
  },
  name: {
    fontFamily: fontFamily.bold,
    fontSize: 20,
    color: colors.text,
    marginBottom: spacing.xs,
  },
  price: {
    fontFamily: fontFamily.bold,
    fontSize: 24,
    color: colors.primary,
    marginBottom: spacing.md,
  },
  feature: {
    color: colors.textMuted,
    fontSize: 14,
    marginBottom: spacing.xs,
    lineHeight: 20,
  },
  cta: {
    marginTop: spacing.md,
  },
});
