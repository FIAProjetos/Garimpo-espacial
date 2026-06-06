import React, { useEffect, useState } from 'react';
import { StyleSheet, Text, TextInput, View } from 'react-native';
import { runClustering } from '../services/api';
import { ApiError } from '../services/api';
import { analyst } from '../theme/analyst';
import { colors } from '../theme/colors';
import { spacing } from '../theme/spacing';
import { fontFamily, typography } from '../theme/typography';
import type {
  ClusterDto,
  ClusteringRequest,
  ClusteringResultDto,
  DebrisDto,
} from '../types/api';
import { Button } from './Button';
import { DataField } from './DataField';
import { ScatterChart } from './ScatterChart';

export const DEFAULT_EPSILON = 0.3;
export const DEFAULT_MIN_POINTS = 5;

type Props = {
  debris: DebrisDto[];
  clusters: ClusterDto[];
  onRunComplete: (result: ClusteringResultDto) => void;
  onParamsChange?: (params: ClusteringRequest) => void;
};

export function ClusteringPanel({
  debris,
  clusters,
  onRunComplete,
  onParamsChange,
}: Props) {
  const [epsilonText, setEpsilonText] = useState(String(DEFAULT_EPSILON));
  const [minPointsText, setMinPointsText] = useState(String(DEFAULT_MIN_POINTS));
  const [running, setRunning] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [lastResult, setLastResult] = useState<ClusteringResultDto | null>(null);

  const parseParams = (): ClusteringRequest | { error: string } => {
    const epsilon = parseFloat(epsilonText.replace(',', '.'));
    const minPoints = parseInt(minPointsText, 10);
    if (Number.isNaN(epsilon) || epsilon <= 0) {
      return { error: 'Epsilon deve ser um número maior que zero.' };
    }
    if (Number.isNaN(minPoints) || minPoints < 1) {
      return { error: 'Min. pontos deve ser um inteiro ≥ 1.' };
    }
    return { epsilon, minPoints };
  };

  useEffect(() => {
    const parsed = parseParams();
    if (!('error' in parsed)) {
      onParamsChange?.(parsed);
    }
  }, [epsilonText, minPointsText, onParamsChange]);

  const handleRun = async () => {
    const parsed = parseParams();
    if ('error' in parsed) {
      setError(parsed.error ?? null);
      return;
    }

    setRunning(true);
    setError(null);
    try {
      const result = await runClustering({
        epsilon: parsed.epsilon,
        minPoints: parsed.minPoints,
      });
      setLastResult(result);
      onRunComplete(result);
    } catch (e) {
      if (e instanceof ApiError && e.status === 400) {
        setError('Parâmetros inválidos. Verifique epsilon e min. pontos.');
      } else {
        setError('Falha ao executar DBSCAN. Verifique a conexão com a API.');
      }
    } finally {
      setRunning(false);
    }
  };

  const chartSubtitle = lastResult
    ? `Execução ε=${lastResult.epsilon}, min=${lastResult.minPoints} · ${new Date(lastResult.completedAt).toLocaleString('pt-BR')}`
    : `Configure ε e min. pontos e execute para ver o mapa`;

  return (
    <View style={styles.panel}>
      <Text style={styles.tag}>EXPERIMENTAÇÃO DBSCAN</Text>
      <Text style={styles.title}>Parâmetros de clusterização</Text>
      <Text style={styles.description}>
        Ajuste o raio de vizinhança (ε, em desvios-padrão) e o número mínimo de pontos para
        formar um núcleo. Cada execução regenera os clusters no catálogo atual.
      </Text>

      <View style={styles.inputs}>
        <View style={styles.inputGroup}>
          <Text style={styles.inputLabel}>Epsilon (ε)</Text>
          <TextInput
            style={styles.input}
            value={epsilonText}
            onChangeText={setEpsilonText}
            keyboardType="decimal-pad"
            placeholder="0.3"
            placeholderTextColor={colors.textMuted}
          />
        </View>
        <View style={styles.inputGroup}>
          <Text style={styles.inputLabel}>Min. pontos</Text>
          <TextInput
            style={styles.input}
            value={minPointsText}
            onChangeText={setMinPointsText}
            keyboardType="number-pad"
            placeholder="5"
            placeholderTextColor={colors.textMuted}
          />
        </View>
        <Button
          label={running ? 'Executando...' : 'Executar DBSCAN'}
          onPress={handleRun}
          loading={running}
          style={styles.runBtn}
        />
      </View>

      {error ? <Text style={styles.error}>{error}</Text> : null}

      {lastResult ? (
        <View style={styles.results}>
          <Text style={styles.resultsTitle}>Resultado da execução</Text>
          <View style={styles.resultsGrid}>
            <DataField label="Processados" value={String(lastResult.processedDebris)} unit="obj" />
            <DataField
              label="Clusters"
              value={String(lastResult.clustersFound)}
              highlight
            />
            <DataField label="Ruído" value={String(lastResult.noiseCount)} unit="obj" />
            <DataField label="Alertas" value={String(lastResult.alertsGenerated)} />
            <DataField label="ε usado" value={String(lastResult.epsilon)} />
            <DataField label="Min. usado" value={String(lastResult.minPoints)} />
          </View>
        </View>
      ) : null}

      <ScatterChart
        debris={debris}
        clusters={clusters}
        title="Mapa da execução"
        subtitle={chartSubtitle}
      />
    </View>
  );
}

const styles = StyleSheet.create({
  panel: {
    backgroundColor: analyst.panelBg,
    borderRadius: 12,
    borderWidth: 1,
    borderColor: analyst.panelBorder,
    padding: spacing.lg,
    marginBottom: spacing.lg,
  },
  tag: {
    ...typography.label,
    color: colors.accentCyan,
    marginBottom: spacing.xs,
  },
  title: {
    fontFamily: fontFamily.bold,
    fontSize: 18,
    color: colors.text,
    marginBottom: spacing.xs,
  },
  description: {
    ...typography.body,
    fontSize: 13,
    lineHeight: 20,
    marginBottom: spacing.md,
  },
  inputs: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: spacing.md,
    alignItems: 'flex-end',
    marginBottom: spacing.md,
  },
  inputGroup: {
    flexGrow: 1,
    minWidth: 120,
  },
  inputLabel: {
    fontFamily: fontFamily.regular,
    fontSize: 10,
    color: colors.textMuted,
    textTransform: 'uppercase',
    letterSpacing: 0.6,
    marginBottom: spacing.xs,
  },
  input: {
    backgroundColor: colors.background,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: analyst.panelBorder,
    color: colors.text,
    fontFamily: fontFamily.medium,
    fontSize: 16,
    padding: spacing.md,
    minHeight: 48,
  },
  runBtn: {
    minWidth: 160,
    flexGrow: 1,
  },
  error: {
    fontFamily: fontFamily.regular,
    color: colors.danger,
    marginBottom: spacing.sm,
    fontSize: 13,
  },
  results: {
    marginBottom: spacing.md,
    paddingTop: spacing.md,
    borderTopWidth: 1,
    borderTopColor: analyst.panelBorder,
  },
  resultsTitle: {
    fontFamily: fontFamily.semiBold,
    fontSize: 13,
    color: colors.text,
    marginBottom: spacing.sm,
  },
  resultsGrid: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: spacing.md,
  },
});
