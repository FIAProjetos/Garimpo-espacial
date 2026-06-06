import React, { useEffect, useState } from 'react';
import { Modal, Pressable, StyleSheet, Text, View } from 'react-native';
import { useNavigation, useNavigationState } from '@react-navigation/native';
import type { NativeStackNavigationProp } from '@react-navigation/native-stack';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { useAuth } from '../hooks/useAuth';
import { useResponsive } from '../hooks/useResponsive';
import { colors } from '../theme/colors';
import { analyst } from '../theme/analyst';
import { spacing } from '../theme/spacing';
import { fontFamily, typography } from '../theme/typography';
import type { MainTabParamList, RootStackParamList } from '../navigation/types';

function useCurrentTab(): keyof MainTabParamList {
  return useNavigationState(state => {
    const route = state.routes[state.index];
    if (route.name !== 'Analyst' || !route.state) return 'Dashboard';
    const tabRoute = route.state.routes[route.state.index ?? 0];
    return (tabRoute?.name ?? 'Dashboard') as keyof MainTabParamList;
  });
}

const TABS: { key: keyof MainTabParamList; label: string }[] = [
  { key: 'Dashboard', label: 'Painel' },
  { key: 'Alerts', label: 'Alertas' },
  { key: 'Profile', label: 'Perfil' },
];

function MissionClock() {
  const [utc, setUtc] = useState('');

  useEffect(() => {
    const tick = () => {
      const now = new Date();
      setUtc(
        now.toISOString().replace('T', ' ').slice(0, 19) + ' UTC',
      );
    };
    tick();
    const id = setInterval(tick, 1000);
    return () => clearInterval(id);
  }, []);

  return <Text style={styles.clock}>{utc}</Text>;
}

export function AnalystNavbar() {
  const navigation = useNavigation<NativeStackNavigationProp<RootStackParamList>>();
  const currentTab = useCurrentTab();
  const insets = useSafeAreaInsets();
  const { isDesktop } = useResponsive();
  const { user } = useAuth();
  const [menuOpen, setMenuOpen] = useState(false);

  const goPublic = () => {
    setMenuOpen(false);
    navigation.navigate('Public', { screen: 'Home' });
  };

  const goTab = (tab: keyof MainTabParamList) => {
    setMenuOpen(false);
    navigation.navigate('Analyst', { screen: tab });
  };

  return (
    <>
      <View style={[styles.bar, { paddingTop: insets.top }]}>
        <View style={[styles.inner, isDesktop && styles.innerDesktop]}>
          <View style={styles.brandRow}>
            <Pressable onPress={() => goTab('Dashboard')}>
              <Text style={styles.logo}>Garimpo · MC</Text>
            </Pressable>
            {isDesktop ? <MissionClock /> : null}
          </View>

          {isDesktop ? (
            <View style={styles.desktopNav}>
              <Pressable onPress={goPublic} style={styles.publicLink}>
                <Text style={styles.publicLinkText}>← Site</Text>
              </Pressable>
              {TABS.map(tab => (
                <Pressable
                  key={tab.key}
                  onPress={() => goTab(tab.key)}
                  style={[styles.navLink, currentTab === tab.key && styles.navLinkActive]}>
                  <Text
                    style={[
                      styles.navLinkText,
                      currentTab === tab.key && styles.navLinkTextActive,
                    ]}>
                    {tab.label}
                  </Text>
                </Pressable>
              ))}
              <View style={styles.userBadge}>
                <View style={styles.statusDot} />
                <Text style={styles.userName} numberOfLines={1}>
                  {user?.fullName ?? 'Analista'}
                </Text>
              </View>
            </View>
          ) : (
            <View style={styles.mobileRight}>
              <View style={styles.userBadgeCompact}>
                <View style={styles.statusDot} />
                <Text style={styles.userNameCompact} numberOfLines={1}>
                  {user?.fullName?.split(' ')[0] ?? 'Analista'}
                </Text>
              </View>
              <Pressable onPress={() => setMenuOpen(true)} style={styles.menuBtn}>
                <View style={styles.menuLine} />
                <View style={styles.menuLine} />
                <View style={styles.menuLine} />
              </Pressable>
            </View>
          )}
        </View>
        {!isDesktop ? (
          <View style={styles.mobileClockRow}>
            <MissionClock />
          </View>
        ) : null}
      </View>

      <Modal visible={menuOpen} transparent animationType="fade">
        <Pressable style={styles.menuOverlay} onPress={() => setMenuOpen(false)}>
          <Pressable
            style={[styles.menuPanel, { paddingTop: insets.top + spacing.lg }]}
            onPress={e => e.stopPropagation()}>
            <Text style={styles.menuTitle}>Mission Control</Text>
            <Pressable onPress={goPublic} style={styles.menuItem}>
              <Text style={styles.menuItemText}>← Voltar ao site</Text>
            </Pressable>
            {TABS.map(tab => (
              <Pressable key={tab.key} onPress={() => goTab(tab.key)} style={styles.menuItem}>
                <Text
                  style={[
                    styles.menuItemText,
                    currentTab === tab.key && styles.menuItemActive,
                  ]}>
                  {tab.label}
                </Text>
              </Pressable>
            ))}
          </Pressable>
        </Pressable>
      </Modal>
    </>
  );
}

const styles = StyleSheet.create({
  bar: {
    backgroundColor: analyst.panelBg,
    borderBottomWidth: 1,
    borderBottomColor: analyst.panelBorder,
    zIndex: 100,
  },
  inner: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingHorizontal: spacing.lg,
    paddingVertical: spacing.sm,
    minHeight: 48,
  },
  innerDesktop: {
    maxWidth: 1200,
    alignSelf: 'center',
    width: '100%',
  },
  brandRow: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.md,
  },
  logo: {
    fontFamily: fontFamily.bold,
    fontSize: 16,
    color: colors.accentCyan,
    letterSpacing: 1,
  },
  clock: {
    ...typography.caption,
    fontFamily: fontFamily.medium,
    color: colors.textMuted,
    letterSpacing: 0.5,
  },
  desktopNav: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.sm,
  },
  publicLink: {
    paddingHorizontal: spacing.sm,
    paddingVertical: spacing.xs,
    marginRight: spacing.sm,
    borderRightWidth: 1,
    borderRightColor: analyst.panelBorder,
  },
  publicLinkText: {
    fontFamily: fontFamily.medium,
    fontSize: 13,
    color: colors.textMuted,
  },
  navLink: {
    paddingHorizontal: spacing.md,
    paddingVertical: spacing.xs,
    borderRadius: 6,
  },
  navLinkActive: {
    backgroundColor: 'rgba(59, 130, 246, 0.15)',
    borderWidth: 1,
    borderColor: 'rgba(59, 130, 246, 0.35)',
  },
  navLinkText: {
    fontFamily: fontFamily.medium,
    fontSize: 14,
    color: colors.textMuted,
  },
  navLinkTextActive: {
    color: colors.accentCyan,
  },
  userBadge: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.xs,
    marginLeft: spacing.md,
    paddingHorizontal: spacing.sm,
    paddingVertical: spacing.xs,
    backgroundColor: colors.surface,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: analyst.panelBorder,
    maxWidth: 160,
  },
  userBadgeCompact: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 4,
    maxWidth: 100,
  },
  statusDot: {
    width: 7,
    height: 7,
    borderRadius: 4,
    backgroundColor: analyst.statusOnline,
  },
  userName: {
    fontFamily: fontFamily.medium,
    fontSize: 12,
    color: colors.text,
  },
  userNameCompact: {
    fontFamily: fontFamily.medium,
    fontSize: 11,
    color: colors.textMuted,
  },
  mobileRight: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.sm,
  },
  mobileClockRow: {
    paddingHorizontal: spacing.lg,
    paddingBottom: spacing.xs,
  },
  menuBtn: {
    padding: spacing.sm,
    gap: 5,
  },
  menuLine: {
    width: 20,
    height: 2,
    backgroundColor: colors.text,
    borderRadius: 1,
  },
  menuOverlay: {
    flex: 1,
    backgroundColor: colors.overlay,
  },
  menuPanel: {
    backgroundColor: analyst.panelBg,
    paddingHorizontal: spacing.lg,
    paddingBottom: spacing.xl,
    borderBottomLeftRadius: 16,
    borderBottomRightRadius: 16,
    borderWidth: 1,
    borderColor: analyst.panelBorder,
  },
  menuTitle: {
    ...typography.label,
    marginBottom: spacing.md,
  },
  menuItem: {
    paddingVertical: spacing.md,
    borderBottomWidth: 1,
    borderBottomColor: analyst.panelBorder,
  },
  menuItemText: {
    fontFamily: fontFamily.medium,
    fontSize: 16,
    color: colors.text,
  },
  menuItemActive: {
    color: colors.accentCyan,
  },
});
