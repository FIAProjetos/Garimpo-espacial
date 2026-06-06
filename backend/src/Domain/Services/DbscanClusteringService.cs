using Garimpo.Domain.ValueObjects;

namespace Garimpo.Domain.Services;

/// <summary>
/// Implementacao pura (sem dependencias externas) do algoritmo DBSCAN
/// (Density-Based Spatial Clustering of Applications with Noise).
///
/// O DBSCAN identifica regioes de alta densidade no espaco de caracteristicas
/// (altitude x inclinacao) e isola outliers (ruido), o que e ideal para mapear
/// aglomerados de lixo espacial sem precisar definir o numero de clusters a priori.
///
/// Como altitude (km) e inclinacao (graus) tem escalas distintas, os pontos sao
/// normalizados por desvio-padrao (z-score) antes do calculo de distancia; assim,
/// <c>epsilon</c> e expresso em desvios-padrao.
/// </summary>
public sealed class DbscanClusteringService
{
    private const int Unclassified = 0;
    public const int NoiseLabel = -1;

    /// <summary>
    /// Executa o DBSCAN sobre os pontos orbitais informados.
    /// </summary>
    /// <param name="points">Pontos no espaco de caracteristicas.</param>
    /// <param name="epsilon">Raio de vizinhanca em desvios-padrao (&gt; 0).</param>
    /// <param name="minPoints">Numero minimo de vizinhos para formar um nucleo (&gt;= 1).</param>
    /// <returns>Mapa de DebrisId para rotulo de cluster (-1 = ruido).</returns>
    public IReadOnlyDictionary<Guid, int> Cluster(
        IReadOnlyList<OrbitalPoint> points,
        double epsilon,
        int minPoints)
    {
        if (epsilon <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(epsilon), "epsilon deve ser maior que zero.");
        }

        if (minPoints < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(minPoints), "minPoints deve ser maior ou igual a 1.");
        }

        var result = new Dictionary<Guid, int>(points.Count);
        if (points.Count == 0)
        {
            return result;
        }

        double[][] normalized = Normalize(points);

        // labels[i]: 0 = nao classificado, -1 = ruido, >0 = id de cluster (1-based interno).
        var labels = new int[points.Count];
        int clusterId = 0;

        for (int i = 0; i < points.Count; i++)
        {
            if (labels[i] != Unclassified)
            {
                continue;
            }

            List<int> neighbors = RegionQuery(normalized, i, epsilon);

            if (neighbors.Count < minPoints)
            {
                labels[i] = NoiseLabel;
                continue;
            }

            clusterId++;
            ExpandCluster(normalized, labels, i, neighbors, clusterId, epsilon, minPoints);
        }

        // Converte o id interno 1-based para rotulo 0-based; mantem -1 para ruido.
        for (int i = 0; i < points.Count; i++)
        {
            int label = labels[i] == NoiseLabel ? NoiseLabel : labels[i] - 1;
            result[points[i].DebrisId] = label;
        }

        return result;
    }

    private static void ExpandCluster(
        double[][] data,
        int[] labels,
        int pointIndex,
        List<int> neighbors,
        int clusterId,
        double epsilon,
        int minPoints)
    {
        labels[pointIndex] = clusterId;

        // Usa indice em vez de foreach pois a lista cresce durante a iteracao.
        for (int n = 0; n < neighbors.Count; n++)
        {
            int neighborIndex = neighbors[n];

            if (labels[neighborIndex] == NoiseLabel)
            {
                // Ruido vira membro de borda do cluster.
                labels[neighborIndex] = clusterId;
            }

            if (labels[neighborIndex] != Unclassified)
            {
                continue;
            }

            labels[neighborIndex] = clusterId;

            List<int> neighborNeighbors = RegionQuery(data, neighborIndex, epsilon);
            if (neighborNeighbors.Count >= minPoints)
            {
                neighbors.AddRange(neighborNeighbors);
            }
        }
    }

    private static List<int> RegionQuery(double[][] data, int pointIndex, double epsilon)
    {
        var neighbors = new List<int>();
        double[] origin = data[pointIndex];

        for (int i = 0; i < data.Length; i++)
        {
            if (EuclideanDistance(origin, data[i]) <= epsilon)
            {
                neighbors.Add(i);
            }
        }

        return neighbors;
    }

    private static double EuclideanDistance(double[] a, double[] b)
    {
        double sum = 0;
        for (int i = 0; i < a.Length; i++)
        {
            double diff = a[i] - b[i];
            sum += diff * diff;
        }

        return Math.Sqrt(sum);
    }

    /// <summary>
    /// Normaliza altitude e inclinacao por z-score para que ambas as dimensoes
    /// contribuam igualmente para a distancia euclidiana.
    /// </summary>
    private static double[][] Normalize(IReadOnlyList<OrbitalPoint> points)
    {
        double altitudeMean = points.Average(p => p.AltitudeKm);
        double inclinationMean = points.Average(p => p.InclinationDegrees);

        double altitudeStdDev = StandardDeviation(points.Select(p => p.AltitudeKm), altitudeMean);
        double inclinationStdDev = StandardDeviation(points.Select(p => p.InclinationDegrees), inclinationMean);

        // Evita divisao por zero quando a dimensao e constante.
        altitudeStdDev = altitudeStdDev == 0 ? 1 : altitudeStdDev;
        inclinationStdDev = inclinationStdDev == 0 ? 1 : inclinationStdDev;

        var normalized = new double[points.Count][];
        for (int i = 0; i < points.Count; i++)
        {
            normalized[i] =
            [
                (points[i].AltitudeKm - altitudeMean) / altitudeStdDev,
                (points[i].InclinationDegrees - inclinationMean) / inclinationStdDev
            ];
        }

        return normalized;
    }

    private static double StandardDeviation(IEnumerable<double> values, double mean)
    {
        var materialized = values as ICollection<double> ?? values.ToList();
        if (materialized.Count == 0)
        {
            return 0;
        }

        double sumSquares = materialized.Sum(v => (v - mean) * (v - mean));
        return Math.Sqrt(sumSquares / materialized.Count);
    }
}
