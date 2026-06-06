import React from 'react';
import { StyleSheet, View, type ViewStyle } from 'react-native';
import { useResponsive } from '../hooks/useResponsive';

type Props = {
  children: React.ReactNode;
  narrow?: boolean;
  style?: ViewStyle;
};

export function ContentContainer({ children, narrow = false, style }: Props) {
  const { contentMaxWidth, narrowMaxWidth, isDesktop } = useResponsive();
  const maxWidth = narrow ? narrowMaxWidth : contentMaxWidth;

  return (
    <View
      style={[
        styles.base,
        isDesktop && { maxWidth, alignSelf: 'center', width: '100%' },
        style,
      ]}>
      {children}
    </View>
  );
}

const styles = StyleSheet.create({
  base: {
    width: '100%',
  },
});
