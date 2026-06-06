import React from 'react';
import { StyleSheet, View } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { colors } from '../theme/colors';
import { PublicNavbar } from './PublicNavbar';

type Props = {
  children: React.ReactNode;
  showNavbar?: boolean;
};

export function PublicLayout({ children, showNavbar = true }: Props) {
  return (
    <SafeAreaView style={styles.safe} edges={['left', 'right']}>
      <View style={styles.container}>
        {showNavbar ? <PublicNavbar /> : null}
        {children}
      </View>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  safe: {
    flex: 1,
    backgroundColor: colors.background,
  },
  container: {
    flex: 1,
  },
});
