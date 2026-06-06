import React from 'react';
import { ScrollView, StyleSheet, Text, View } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { Button } from '../components/Button';
import { DataField } from '../components/DataField';
import { useAnalystNavigation } from '../hooks/useAppNavigation';
import { useAuth } from '../hooks/useAuth';
import { analyst } from '../theme/analyst';
import { colors } from '../theme/colors';
import { spacing } from '../theme/spacing';
import { fontFamily, typography } from '../theme/typography';

export function ProfileScreen() {
  const { user, logout } = useAuth();
  const navigation = useAnalystNavigation();

  return (
    <SafeAreaView style={styles.safe} edges={['left', 'right', 'bottom']}>
      <ScrollView
        style={styles.scroll}
        contentContainerStyle={styles.content}
        showsVerticalScrollIndicator={false}>
        <Text style={styles.tag}>CONTA DE ANALISTA</Text>
        <Text style={styles.title}>Perfil operacional</Text>
        <Text style={styles.subtitle}>Credenciais e acesso ao mission control.</Text>

        <View style={styles.card}>
          <View style={styles.statusRow}>
            <View style={styles.statusDot} />
            <Text style={styles.statusText}>Sessão ativa</Text>
          </View>
          <View style={styles.grid}>
            <DataField label="Nome" value={user?.fullName ?? '—'} highlight />
            <DataField label="E-mail" value={user?.email ?? '—'} />
            <DataField label="Papel" value={user?.role ?? '—'} />
            <DataField
              label="Membro desde"
              value={
                user?.createdAt
                  ? new Date(user.createdAt).toLocaleDateString('pt-BR')
                  : '—'
              }
            />
          </View>
        </View>

        <Button
          label="Voltar ao site"
          onPress={() => navigation.navigate('Public', { screen: 'Home' })}
          variant="ghost"
          style={styles.btn}
        />
        <Button label="Encerrar sessão" onPress={logout} variant="ghost" style={styles.btn} />
      </ScrollView>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  safe: {
    flex: 1,
    backgroundColor: colors.background,
  },
  scroll: {
    flex: 1,
    backgroundColor: colors.background,
  },
  content: {
    padding: spacing.lg,
    paddingTop: spacing.md,
    maxWidth: 640,
    alignSelf: 'center',
    width: '100%',
    paddingBottom: spacing.xl,
  },
  tag: {
    ...typography.label,
    color: colors.accentCyan,
    marginBottom: spacing.xs,
  },
  title: {
    fontFamily: fontFamily.bold,
    fontSize: 22,
    color: colors.text,
  },
  subtitle: {
    ...typography.body,
    marginBottom: spacing.lg,
  },
  card: {
    backgroundColor: analyst.panelBg,
    borderRadius: 12,
    borderWidth: 1,
    borderColor: analyst.panelBorder,
    padding: spacing.lg,
    marginBottom: spacing.lg,
  },
  statusRow: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.sm,
    marginBottom: spacing.md,
    paddingBottom: spacing.md,
    borderBottomWidth: 1,
    borderBottomColor: analyst.panelBorder,
  },
  statusDot: {
    width: 8,
    height: 8,
    borderRadius: 4,
    backgroundColor: colors.success,
  },
  statusText: {
    fontFamily: fontFamily.medium,
    fontSize: 13,
    color: colors.success,
  },
  grid: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: spacing.lg,
  },
  btn: {
    marginBottom: spacing.sm,
  },
});
