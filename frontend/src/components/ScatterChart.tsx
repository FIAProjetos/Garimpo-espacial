import React, { useMemo } from 'react';
import { StyleSheet, Text, useWindowDimensions, View } from 'react-native';
import Svg, { Circle, G, Line, Rect, Text as SvgText } from 'react-native-svg';
import { useResponsive } from '../hooks/useResponsive';
import { analyst } from '../theme/analyst';
import { colors } from '../theme/colors';
import { spacing } from '../theme/spacing';
import { fontFamily } from '../theme/typography';
import type { ClusterDto, DebrisDto } from '../types/api';

const PADDING_LEFT = 48;
const PADDING_RIGHT = 16;
const PADDING_TOP = 24;
const PADDING_BOTTOM = 40;
const MOBILE_HEIGHT = 260;
const DESKTOP_HEIGHT = 340;
const DESKTOP_MAX_WIDTH = 680;

const ALT_TICKS = [0, 500, 1000, 1500, 2000];
const INC_TICKS = [0, 45, 90, 135, 180];
const CLUSTER_COLORS = ['#3B82F6', '#22C55E', '#F59E0B', '#A855F7', '#EC4899', '#14B8A6'];

type Props = {
  debris: DebrisDto[];
  clusters: ClusterDto[];
  title?: string;
  subtitle?: string;
};

function scale(value: number, min: number, max: number, size: number): number {
  if (max === min) return size / 2;
  return ((value - min) / (max - min)) * size;
}

export function ScatterChart({
  debris,
  clusters,
  title = 'Mapa de densidade orbital',
  subtitle = 'Distribuição LEO · Altitude (km) × Inclinação (°)',
}: Props) {
  const { width: windowWidth } = useWindowDimensions();
  const { isDesktop } = useResponsive();

  const chartWidth = useMemo(() => {
    const horizontalPadding = spacing.lg * 4;
    const maxWidth = isDesktop ? DESKTOP_MAX_WIDTH : windowWidth - horizontalPadding;
    return Math.max(300, Math.min(maxWidth, windowWidth - horizontalPadding));
  }, [isDesktop, windowWidth]);

  const chartHeight = isDesktop ? DESKTOP_HEIGHT : MOBILE_HEIGHT;

  const plot = useMemo(() => {
    const innerW = chartWidth - PADDING_LEFT - PADDING_RIGHT;
    const innerH = chartHeight - PADDING_TOP - PADDING_BOTTOM;
    const altMin = 0;
    const altMax = 2000;
    const incMin = 0;
    const incMax = 180;

    const clusterColorMap = new Map<string, string>();
    clusters.forEach((c, i) => {
      clusterColorMap.set(c.id, CLUSTER_COLORS[i % CLUSTER_COLORS.length]);
    });

    const gridH = ALT_TICKS.map(v => ({
      y: PADDING_TOP + innerH - scale(v, altMin, altMax, innerH),
      label: String(v),
      major: v % 1000 === 0,
    }));

    const gridV = INC_TICKS.map(v => ({
      x: PADDING_LEFT + scale(v, incMin, incMax, innerW),
      label: String(v),
      major: v % 90 === 0,
    }));

    const points = debris.map(d => ({
      x: PADDING_LEFT + scale(d.altitudeKm, altMin, altMax, innerW),
      y: PADDING_TOP + innerH - scale(d.inclinationDegrees, incMin, incMax, innerH),
      color: d.clusterId
        ? clusterColorMap.get(d.clusterId) ?? colors.textMuted
        : colors.textMuted,
      r: d.clusterId ? 3.5 : 2,
      clustered: !!d.clusterId,
    }));

    const centroids = clusters.map((c, i) => ({
      x: PADDING_LEFT + scale(c.centroidAltitudeKm, altMin, altMax, innerW),
      y:
        PADDING_TOP +
        innerH -
        scale(c.centroidInclinationDegrees, incMin, incMax, innerH),
      color: CLUSTER_COLORS[i % CLUSTER_COLORS.length],
    }));

    const clustered = points.filter(p => p.clustered).length;
    const unclustered = points.length - clustered;

    return { points, centroids, gridH, gridV, innerW, innerH, clustered, unclustered };
  }, [debris, clusters, chartWidth, chartHeight]);

  if (debris.length === 0) {
    return (
      <View style={styles.empty}>
        <Text style={styles.chartTitle}>{title}</Text>
        <Text style={styles.emptyText}>
          Execute a ingestão TLE para gerar o scatter plot altitude × inclinação.
        </Text>
      </View>
    );
  }

  return (
    <View style={[styles.container, isDesktop && styles.containerDesktop]}>
      <View style={styles.header}>
        <View>
          <Text style={styles.chartTitle}>{title}</Text>
          <Text style={styles.chartSubtitle}>{subtitle}</Text>
        </View>
        <View style={styles.stats}>
          <Text style={styles.statText}>n={debris.length}</Text>
          <Text style={styles.statText}>k={clusters.length}</Text>
        </View>
      </View>

      <Svg width={chartWidth} height={chartHeight}>
        <Rect
          x={PADDING_LEFT}
          y={PADDING_TOP}
          width={plot.innerW}
          height={plot.innerH}
          fill="rgba(15, 20, 32, 0.6)"
          stroke={analyst.panelBorder}
          strokeWidth={1}
        />

        {plot.gridH.map((g, i) => (
          <G key={`gh-${i}`}>
            <Line
              x1={PADDING_LEFT}
              y1={g.y}
              x2={PADDING_LEFT + plot.innerW}
              y2={g.y}
              stroke={g.major ? colors.gridLineMajor : colors.gridLine}
              strokeDasharray={g.major ? undefined : '4,4'}
            />
            <SvgText
              x={PADDING_LEFT - 6}
              y={g.y + 3}
              fill={colors.textMuted}
              fontSize="9"
              textAnchor="end">
              {g.label}
            </SvgText>
          </G>
        ))}

        {plot.gridV.map((g, i) => (
          <G key={`gv-${i}`}>
            <Line
              x1={g.x}
              y1={PADDING_TOP}
              x2={g.x}
              y2={PADDING_TOP + plot.innerH}
              stroke={g.major ? colors.gridLineMajor : colors.gridLine}
              strokeDasharray={g.major ? undefined : '4,4'}
            />
            <SvgText
              x={g.x}
              y={PADDING_TOP + plot.innerH + 14}
              fill={colors.textMuted}
              fontSize="9"
              textAnchor="middle">
              {g.label}
            </SvgText>
          </G>
        ))}

        <Line
          x1={PADDING_LEFT}
          y1={PADDING_TOP + plot.innerH}
          x2={PADDING_LEFT + plot.innerW}
          y2={PADDING_TOP + plot.innerH}
          stroke={colors.textMuted}
          strokeWidth={1.5}
        />
        <Line
          x1={PADDING_LEFT}
          y1={PADDING_TOP}
          x2={PADDING_LEFT}
          y2={PADDING_TOP + plot.innerH}
          stroke={colors.textMuted}
          strokeWidth={1.5}
        />

        {plot.points.map((p, i) => (
          <Circle key={`d-${i}`} cx={p.x} cy={p.y} r={p.r} fill={p.color} opacity={0.75} />
        ))}
        {plot.centroids.map((c, i) => (
          <Circle
            key={`c-${i}`}
            cx={c.x}
            cy={c.y}
            r={8}
            fill="none"
            stroke={c.color}
            strokeWidth={2}
          />
        ))}

        <SvgText
          x={PADDING_LEFT + plot.innerW / 2}
          y={chartHeight - 6}
          fill={colors.textMuted}
          fontSize="10"
          textAnchor="middle">
          Altitude (km)
        </SvgText>
        <SvgText
          x={12}
          y={PADDING_TOP + plot.innerH / 2}
          fill={colors.textMuted}
          fontSize="10"
          textAnchor="middle"
          rotation="-90"
          origin={`12, ${PADDING_TOP + plot.innerH / 2}`}>
          Inclinação (°)
        </SvgText>
      </Svg>

      <View style={styles.legend}>
        <LegendItem color={colors.textMuted} label={`Detritos (${plot.unclustered})`} dot />
        <LegendItem color={colors.primary} label={`Em cluster (${plot.clustered})`} dot />
        <LegendItem color={colors.secondary} label="Centróide DBSCAN" ring />
      </View>
    </View>
  );
}

function LegendItem({
  color,
  label,
  dot,
  ring,
}: {
  color: string;
  label: string;
  dot?: boolean;
  ring?: boolean;
}) {
  return (
    <View style={styles.legendItem}>
      {ring ? (
        <View style={[styles.legendRing, { borderColor: color }]} />
      ) : (
        <View style={[styles.legendDot, { backgroundColor: color }]} />
      )}
      <Text style={styles.legendText}>{label}</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    backgroundColor: analyst.panelBg,
    borderRadius: 12,
    borderWidth: 1,
    borderColor: analyst.panelBorder,
    padding: spacing.md,
    marginBottom: spacing.md,
    flex: 1,
  },
  containerDesktop: {
    marginBottom: 0,
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
    marginBottom: spacing.sm,
  },
  chartTitle: {
    fontFamily: fontFamily.semiBold,
    fontSize: 14,
    color: colors.text,
    letterSpacing: 0.3,
  },
  chartSubtitle: {
    fontFamily: fontFamily.regular,
    fontSize: 11,
    color: colors.textMuted,
    marginTop: 2,
  },
  stats: {
    alignItems: 'flex-end',
    gap: 2,
  },
  statText: {
    fontFamily: fontFamily.medium,
    fontSize: 11,
    color: colors.accentCyan,
    letterSpacing: 0.5,
  },
  legend: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: spacing.md,
    marginTop: spacing.sm,
    paddingTop: spacing.sm,
    borderTopWidth: 1,
    borderTopColor: analyst.panelBorder,
  },
  legendItem: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.xs,
  },
  legendDot: {
    width: 8,
    height: 8,
    borderRadius: 4,
  },
  legendRing: {
    width: 10,
    height: 10,
    borderRadius: 5,
    borderWidth: 2,
  },
  legendText: {
    fontFamily: fontFamily.regular,
    fontSize: 11,
    color: colors.textMuted,
  },
  empty: {
    backgroundColor: analyst.panelBg,
    borderRadius: 12,
    borderWidth: 1,
    borderColor: analyst.panelBorder,
    padding: spacing.lg,
    marginBottom: spacing.md,
    flex: 1,
  },
  emptyText: {
    fontFamily: fontFamily.regular,
    color: colors.textMuted,
    textAlign: 'center',
    lineHeight: 20,
    marginTop: spacing.sm,
  },
});
