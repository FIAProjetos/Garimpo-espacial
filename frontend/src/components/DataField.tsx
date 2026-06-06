import React from 'react';
import { StyleSheet, Text, View } from 'react-native';
import { colors } from '../theme/colors';
import { spacing } from '../theme/spacing';
import { fontFamily } from '../theme/typography';

type Props = {
  label: string;
  value: string;
  unit?: string;
  highlight?: boolean;
};

export function DataField({ label, value, unit, highlight }: Props) {
  return (
    <View style={styles.field}>
      <Text style={styles.label}>{label}</Text>
      <Text style={[styles.value, highlight && styles.valueHighlight]}>
        {value}
        {unit ? <Text style={styles.unit}> {unit}</Text> : null}
      </Text>
    </View>
  );
}

const styles = StyleSheet.create({
  field: {
    minWidth: 80,
  },
  label: {
    fontFamily: fontFamily.regular,
    fontSize: 10,
    color: colors.textMuted,
    textTransform: 'uppercase',
    letterSpacing: 0.6,
    marginBottom: 2,
  },
  value: {
    fontFamily: fontFamily.semiBold,
    fontSize: 14,
    color: colors.text,
  },
  valueHighlight: {
    color: colors.accentCyan,
  },
  unit: {
    fontFamily: fontFamily.regular,
    fontSize: 11,
    color: colors.textMuted,
  },
});
