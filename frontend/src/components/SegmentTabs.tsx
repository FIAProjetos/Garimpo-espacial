import React from 'react';
import { Pressable, StyleSheet, Text, View } from 'react-native';
import { analyst } from '../theme/analyst';
import { colors } from '../theme/colors';
import { spacing } from '../theme/spacing';
import { fontFamily } from '../theme/typography';

type Props<T extends string> = {
  options: { key: T; label: string }[];
  value: T;
  onChange: (value: T) => void;
};

export function SegmentTabs<T extends string>({ options, value, onChange }: Props<T>) {
  return (
    <View style={styles.container}>
      {options.map(option => {
        const active = option.key === value;
        return (
          <Pressable
            key={option.key}
            onPress={() => onChange(option.key)}
            style={[styles.tab, active && styles.tabActive]}>
            <Text style={[styles.label, active && styles.labelActive]}>{option.label}</Text>
          </Pressable>
        );
      })}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flexDirection: 'row',
    backgroundColor: analyst.panelBg,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: analyst.panelBorder,
    padding: spacing.xs,
    marginBottom: spacing.md,
  },
  tab: {
    flex: 1,
    paddingVertical: spacing.sm,
    alignItems: 'center',
    borderRadius: 6,
  },
  tabActive: {
    backgroundColor: 'rgba(59, 130, 246, 0.2)',
    borderWidth: 1,
    borderColor: 'rgba(59, 130, 246, 0.4)',
  },
  label: {
    fontFamily: fontFamily.medium,
    fontSize: 13,
    color: colors.textMuted,
    letterSpacing: 0.3,
  },
  labelActive: {
    color: colors.accentCyan,
  },
});
