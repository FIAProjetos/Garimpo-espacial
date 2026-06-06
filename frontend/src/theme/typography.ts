import { TextStyle } from 'react-native';
import { colors } from './colors';

export const fontFamily = {
  regular: 'Exo2_400Regular',
  medium: 'Exo2_500Medium',
  semiBold: 'Exo2_600SemiBold',
  bold: 'Exo2_700Bold',
};

export const typography: Record<string, TextStyle> = {
  display: {
    fontFamily: fontFamily.bold,
    fontSize: 40,
    lineHeight: 48,
    color: colors.text,
    letterSpacing: -0.5,
  },
  h1: {
    fontFamily: fontFamily.bold,
    fontSize: 32,
    lineHeight: 40,
    color: colors.text,
  },
  h2: {
    fontFamily: fontFamily.semiBold,
    fontSize: 24,
    lineHeight: 32,
    color: colors.text,
  },
  h3: {
    fontFamily: fontFamily.semiBold,
    fontSize: 18,
    lineHeight: 26,
    color: colors.text,
  },
  body: {
    fontFamily: fontFamily.regular,
    fontSize: 16,
    lineHeight: 24,
    color: colors.textMuted,
  },
  bodyStrong: {
    fontFamily: fontFamily.medium,
    fontSize: 16,
    lineHeight: 24,
    color: colors.text,
  },
  caption: {
    fontFamily: fontFamily.regular,
    fontSize: 13,
    lineHeight: 18,
    color: colors.textMuted,
  },
  label: {
    fontFamily: fontFamily.semiBold,
    fontSize: 12,
    lineHeight: 16,
    color: colors.accentCyan,
    letterSpacing: 1,
    textTransform: 'uppercase',
  },
};
