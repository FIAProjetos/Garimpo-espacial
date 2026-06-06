import React from 'react';
import { Modal, Pressable, StyleSheet, View, type ViewStyle } from 'react-native';
import { useResponsive } from '../hooks/useResponsive';
import { colors } from '../theme/colors';
import { spacing } from '../theme/spacing';

type Props = {
  visible: boolean;
  onClose: () => void;
  children: React.ReactNode;
  cardStyle?: ViewStyle;
};

export function AuthModalShell({ visible, onClose, children, cardStyle }: Props) {
  const { isDesktop, modalMaxWidth } = useResponsive();

  return (
    <Modal visible={visible} transparent animationType="fade">
      <Pressable style={styles.overlay} onPress={onClose}>
        <Pressable
          style={[
            styles.card,
            isDesktop && { maxWidth: modalMaxWidth, width: '100%', alignSelf: 'center' },
            cardStyle,
          ]}
          onPress={e => e.stopPropagation()}>
          {children}
        </Pressable>
      </Pressable>
    </Modal>
  );
}

const styles = StyleSheet.create({
  overlay: {
    flex: 1,
    backgroundColor: colors.overlay,
    justifyContent: 'center',
    padding: spacing.lg,
  },
  card: {
    backgroundColor: colors.surface,
    borderRadius: 16,
    padding: spacing.lg,
    borderWidth: 1,
    borderColor: colors.surfaceBorder,
  },
});
