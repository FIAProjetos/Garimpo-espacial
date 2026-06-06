import React, { useState } from 'react';
import { Pressable, StyleSheet, Text, TextInput } from 'react-native';
import { useAuth } from '../hooks/useAuth';
import { ApiError } from '../services/api';
import { colors } from '../theme/colors';
import { spacing } from '../theme/spacing';
import { AuthModalShell } from './AuthModalShell';
import { Button } from './Button';

export function RegisterModal() {
  const { registerModalVisible, closeRegisterModal, openLoginModal, register } = useAuth();
  const [fullName, setFullName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const resetForm = () => {
    setFullName('');
    setEmail('');
    setPassword('');
    setError(null);
  };

  const handleRegister = async () => {
    if (!fullName.trim() || !email.trim() || !password) {
      setError('Preencha todos os campos.');
      return;
    }

    setLoading(true);
    setError(null);
    try {
      await register({
        fullName: fullName.trim(),
        email: email.trim().toLowerCase(),
        password,
      });
      resetForm();
    } catch (e) {
      if (e instanceof ApiError && e.status === 409) {
        setError('Este e-mail já está cadastrado.');
      } else if (e instanceof ApiError && e.status === 400) {
        setError('Dados inválidos. Verifique os campos.');
      } else {
        setError('Não foi possível criar a conta. Verifique a conexão com a API.');
      }
    } finally {
      setLoading(false);
    }
  };

  const switchToLogin = () => {
    setError(null);
    openLoginModal();
  };

  return (
    <AuthModalShell visible={registerModalVisible} onClose={closeRegisterModal}>
      <Text style={styles.title}>Criar conta grátis</Text>
      <Text style={styles.subtitle}>
        Cadastre-se no plano Beta e acesse o painel de análise de clusters.
      </Text>
      <TextInput
        style={styles.input}
        placeholder="Nome completo"
        placeholderTextColor={colors.textMuted}
        value={fullName}
        onChangeText={setFullName}
      />
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
      <Button label="Criar conta" onPress={handleRegister} loading={loading} />
      <Pressable onPress={switchToLogin} style={styles.switch}>
        <Text style={styles.switchText}>
          Já tem conta? <Text style={styles.switchLink}>Entrar</Text>
        </Text>
      </Pressable>
      <Pressable onPress={closeRegisterModal} style={styles.close}>
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
    lineHeight: 20,
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
