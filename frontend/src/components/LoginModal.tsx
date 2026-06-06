import React, { useState } from 'react';
import { Pressable, StyleSheet, Text, TextInput } from 'react-native';
import { useAuth } from '../hooks/useAuth';
import { ApiError } from '../services/api';
import { colors } from '../theme/colors';
import { spacing } from '../theme/spacing';
import { AuthModalShell } from './AuthModalShell';
import { Button } from './Button';

export function LoginModal() {
  const { loginModalVisible, closeLoginModal, openRegisterModal, login } = useAuth();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const handleLogin = async () => {
    setLoading(true);
    setError(null);
    try {
      await login(email.trim(), password);
      setEmail('');
      setPassword('');
    } catch (e) {
      if (e instanceof ApiError && e.status === 401) {
        setError('Credenciais inválidas.');
      } else {
        setError('Não foi possível entrar. Verifique a conexão com a API.');
      }
    } finally {
      setLoading(false);
    }
  };

  const switchToRegister = () => {
    setError(null);
    openRegisterModal();
  };

  return (
    <AuthModalShell visible={loginModalVisible} onClose={closeLoginModal}>
      <Text style={styles.title}>Entrar</Text>
      <Text style={styles.subtitle}>Acesse o painel de análise orbital.</Text>
      <TextInput
        style={styles.input}
        placeholder="E-mail"
        placeholderTextColor={colors.textMuted}
        value={email}
        onChangeText={setEmail}
        autoCapitalize="none"
        keyboardType="email-address"
      />
      <TextInput
        style={styles.input}
        placeholder="Senha"
        placeholderTextColor={colors.textMuted}
        value={password}
        onChangeText={setPassword}
        secureTextEntry
      />
      {error ? <Text style={styles.error}>{error}</Text> : null}
      <Button label="Entrar" onPress={handleLogin} loading={loading} />
      <Pressable onPress={switchToRegister} style={styles.switch}>
        <Text style={styles.switchText}>
          Não tem conta? <Text style={styles.switchLink}>Criar conta grátis</Text>
        </Text>
      </Pressable>
      <Pressable onPress={closeLoginModal} style={styles.close}>
        <Text style={styles.closeText}>Fechar</Text>
      </Pressable>
    </AuthModalShell>
  );
}

const styles = StyleSheet.create({
  title: {
    fontSize: 22,
    fontWeight: '700',
    color: colors.text,
    marginBottom: spacing.xs,
  },
  subtitle: {
    fontSize: 14,
    color: colors.textMuted,
    marginBottom: spacing.md,
  },
  input: {
    backgroundColor: colors.background,
    borderRadius: 10,
    borderWidth: 1,
    borderColor: colors.surfaceBorder,
    color: colors.text,
    padding: spacing.md,
    marginBottom: spacing.sm,
  },
  error: {
    color: colors.danger,
    marginBottom: spacing.sm,
  },
  switch: {
    marginTop: spacing.md,
    alignItems: 'center',
  },
  switchText: {
    color: colors.textMuted,
    fontSize: 14,
  },
  switchLink: {
    color: colors.primary,
    fontWeight: '600',
  },
  close: {
    marginTop: spacing.sm,
    alignItems: 'center',
  },
  closeText: {
    color: colors.textMuted,
  },
});
