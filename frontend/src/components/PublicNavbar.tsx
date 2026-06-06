import React, { useState } from 'react';
import { Modal, Pressable, StyleSheet, Text, View } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { usePublicNavigation } from '../hooks/useAppNavigation';
import { useAuth } from '../hooks/useAuth';
import { useResponsive } from '../hooks/useResponsive';
import { colors } from '../theme/colors';
import { spacing } from '../theme/spacing';
import { fontFamily } from '../theme/typography';
import { Button } from './Button';

export function PublicNavbar() {
  const navigation = usePublicNavigation();
  const insets = useSafeAreaInsets();
  const { isDesktop } = useResponsive();
  const { isAuthenticated, user, openLoginModal, openRegisterModal } = useAuth();
  const [menuOpen, setMenuOpen] = useState(false);

  const goHome = () => {
    setMenuOpen(false);
    navigation.navigate('Home');
  };

  const goPricing = () => {
    setMenuOpen(false);
    navigation.navigate('Pricing');
  };

  const goPanel = () => {
    setMenuOpen(false);
    navigation.navigate('Analyst', { screen: 'Dashboard' });
  };

  const handleLogin = () => {
    setMenuOpen(false);
    openLoginModal();
  };

  const handleRegister = () => {
    setMenuOpen(false);
    openRegisterModal();
  };

  const authActions = isAuthenticated ? (
    <View style={styles.authRow}>
      <View style={styles.loggedBadge}>
        <View style={styles.statusDot} />
        <Text style={styles.loggedText} numberOfLines={1}>
          {user?.fullName ?? user?.email}
        </Text>
      </View>
      <Button label="Painel" onPress={goPanel} style={styles.navBtn} />
    </View>
  ) : (
    <View style={styles.desktopActions}>
      <Button label="Entrar" onPress={handleLogin} variant="ghost" style={styles.navBtn} />
      <Button label="Criar conta grátis" onPress={handleRegister} style={styles.navBtn} />
    </View>
  );

  return (
    <>
      <View style={[styles.bar, { paddingTop: insets.top + spacing.sm }]}>
        <View style={[styles.inner, isDesktop && styles.innerDesktop]}>
          <Pressable onPress={goHome}>
            <Text style={styles.logo}>Garimpo Espacial</Text>
          </Pressable>

          {isDesktop ? (
            <View style={styles.desktopNav}>
              <Pressable onPress={goHome} style={styles.navLink}>
                <Text style={styles.navLinkText}>Início</Text>
              </Pressable>
              <Pressable onPress={goPricing} style={styles.navLink}>
                <Text style={styles.navLinkText}>Planos</Text>
              </Pressable>
              {authActions}
            </View>
          ) : (
            <Pressable onPress={() => setMenuOpen(true)} style={styles.menuBtn}>
              <View style={styles.menuLine} />
              <View style={styles.menuLine} />
              <View style={styles.menuLine} />
            </Pressable>
          )}
        </View>
      </View>

      <Modal visible={menuOpen} transparent animationType="fade">
        <Pressable style={styles.menuOverlay} onPress={() => setMenuOpen(false)}>
          <Pressable
            style={[styles.menuPanel, { paddingTop: insets.top + spacing.lg }]}
            onPress={e => e.stopPropagation()}>
            <Text style={styles.menuTitle}>Menu</Text>
            <Pressable onPress={goHome} style={styles.menuItem}>
              <Text style={styles.menuItemText}>Início</Text>
            </Pressable>
            <Pressable onPress={goPricing} style={styles.menuItem}>
              <Text style={styles.menuItemText}>Planos</Text>
            </Pressable>
            {isAuthenticated ? (
              <>
                <View style={styles.menuLogged}>
                  <View style={styles.statusDot} />
                  <Text style={styles.menuLoggedText}>{user?.fullName}</Text>
                </View>
                <Button label="Acessar painel" onPress={goPanel} style={styles.menuCta} />
              </>
            ) : (
              <>
                <Pressable onPress={handleLogin} style={styles.menuItem}>
                  <Text style={styles.menuItemText}>Entrar</Text>
                </Pressable>
                <Button label="Criar conta grátis" onPress={handleRegister} style={styles.menuCta} />
              </>
            )}
          </Pressable>
        </Pressable>
      </Modal>
    </>
  );
}

const styles = StyleSheet.create({
  bar: {
    position: 'absolute',
    top: 0,
    left: 0,
    right: 0,
    zIndex: 100,
    backgroundColor: colors.navbarBg,
    borderBottomWidth: 1,
    borderBottomColor: colors.surfaceBorder,
  },
  inner: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingHorizontal: spacing.lg,
    paddingBottom: spacing.sm,
  },
  innerDesktop: {
    maxWidth: 1080,
    alignSelf: 'center',
    width: '100%',
  },
  logo: {
    fontFamily: fontFamily.bold,
    fontSize: 18,
    color: colors.text,
    letterSpacing: 0.5,
  },
  desktopNav: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.lg,
  },
  navLink: {
    paddingVertical: spacing.xs,
    paddingHorizontal: spacing.sm,
  },
  navLinkText: {
    fontFamily: fontFamily.medium,
    fontSize: 15,
    color: colors.textMuted,
  },
  desktopActions: {
    flexDirection: 'row',
    gap: spacing.sm,
    marginLeft: spacing.md,
  },
  authRow: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.sm,
    marginLeft: spacing.md,
  },
  loggedBadge: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.xs,
    paddingHorizontal: spacing.sm,
    paddingVertical: spacing.xs,
    backgroundColor: colors.surface,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: colors.surfaceBorder,
    maxWidth: 180,
  },
  statusDot: {
    width: 7,
    height: 7,
    borderRadius: 4,
    backgroundColor: colors.success,
  },
  loggedText: {
    fontFamily: fontFamily.medium,
    fontSize: 12,
    color: colors.text,
  },
  navBtn: {
    minHeight: 40,
    paddingVertical: spacing.sm,
    paddingHorizontal: spacing.md,
  },
  menuBtn: {
    padding: spacing.sm,
    gap: 5,
  },
  menuLine: {
    width: 22,
    height: 2,
    backgroundColor: colors.text,
    borderRadius: 1,
  },
  menuOverlay: {
    flex: 1,
    backgroundColor: colors.overlay,
  },
  menuPanel: {
    backgroundColor: colors.surface,
    paddingHorizontal: spacing.lg,
    paddingBottom: spacing.xl,
    borderBottomLeftRadius: 16,
    borderBottomRightRadius: 16,
  },
  menuTitle: {
    fontFamily: fontFamily.bold,
    fontSize: 20,
    color: colors.text,
    marginBottom: spacing.md,
  },
  menuItem: {
    paddingVertical: spacing.md,
    borderBottomWidth: 1,
    borderBottomColor: colors.surfaceBorder,
  },
  menuItemText: {
    fontFamily: fontFamily.medium,
    fontSize: 17,
    color: colors.text,
  },
  menuLogged: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.sm,
    paddingVertical: spacing.md,
  },
  menuLoggedText: {
    fontFamily: fontFamily.medium,
    fontSize: 14,
    color: colors.accentCyan,
  },
  menuCta: {
    marginTop: spacing.lg,
  },
});
