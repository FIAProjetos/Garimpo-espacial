import React from 'react';
import { Pressable, StyleSheet, Text, View } from 'react-native';
import { useNavigation } from '@react-navigation/native';
import type { NativeStackNavigationProp } from '@react-navigation/native-stack';
import { colors } from '../../theme/colors';
import { spacing } from '../../theme/spacing';
import { fontFamily, typography } from '../../theme/typography';
import type { PublicStackParamList } from '../../navigation/types';
import { ContentContainer } from '../ContentContainer';

type Nav = NativeStackNavigationProp<PublicStackParamList>;

export function PublicFooter() {
  const navigation = useNavigation<Nav>();

  return (
    <View style={styles.footer}>
      <ContentContainer>
        <View style={styles.row}>
          <View>
            <Text style={styles.brand}>Garimpo Espacial</Text>
            <Text style={styles.tagline}>Global Solution · FIAP 2026</Text>
          </View>
          <View style={styles.links}>
            <Pressable onPress={() => navigation.navigate('Home')}>
              <Text style={styles.link}>Início</Text>
            </Pressable>
            <Pressable onPress={() => navigation.navigate('Pricing')}>
              <Text style={styles.link}>Planos</Text>
            </Pressable>
          </View>
        </View>
        <Text style={styles.copy}>
          Imagens: NASA / Wikimedia Commons (domínio público). Dados TLE de fontes abertas.
        </Text>
      </ContentContainer>
    </View>
  );
}

const styles = StyleSheet.create({
  footer: {
    paddingVertical: spacing.xl,
    paddingHorizontal: spacing.lg,
    borderTopWidth: 1,
    borderTopColor: colors.surfaceBorder,
    backgroundColor: colors.surface,
  },
  row: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
    marginBottom: spacing.lg,
    flexWrap: 'wrap',
    gap: spacing.md,
  },
  brand: {
    fontFamily: fontFamily.bold,
    fontSize: 16,
    color: colors.text,
    marginBottom: spacing.xs,
  },
  tagline: {
    ...typography.caption,
  },
  links: {
    gap: spacing.sm,
  },
  link: {
    fontFamily: fontFamily.medium,
    fontSize: 14,
    color: colors.textMuted,
    marginBottom: spacing.xs,
  },
  copy: {
    ...typography.caption,
    fontSize: 11,
    opacity: 0.7,
  },
});
