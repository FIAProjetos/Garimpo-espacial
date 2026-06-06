using System.Globalization;
using Garimpo.Domain.Entities;
using Garimpo.Domain.Exceptions;
using Garimpo.Domain.ValueObjects;

namespace Garimpo.Infrastructure.Tle;

/// <summary>
/// Interpreta o formato TLE (Two-Line Element) em tres linhas (nome + linha 1 + linha 2)
/// produzindo entidades de dominio <see cref="Debris"/>.
///
/// Layout das colunas (1-indexado) conforme especificacao NORAD:
///   Linha 1: cols 03-07 = numero do catalogo (NORAD ID)
///   Linha 2: cols 09-16 = inclinacao (graus)
///            cols 18-25 = RAAN (graus)
///            cols 27-33 = excentricidade (com ponto decimal implicito)
///            cols 53-63 = movimento medio (rev/dia)
/// </summary>
public sealed class TleParser
{
    /// <summary>
    /// Converte um bloco textual TLE (varios objetos) em detritos. Registros invalidos
    /// sao ignorados para nao interromper o pipeline de ingestao.
    /// </summary>
    public IReadOnlyList<Debris> Parse(string rawContent, DateTime capturedAt)
    {
        if (string.IsNullOrWhiteSpace(rawContent))
        {
            return Array.Empty<Debris>();
        }

        var lines = rawContent
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.TrimEnd('\r'))
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .ToList();

        var result = new List<Debris>();

        // Cada objeto ocupa 3 linhas: nome, linha 1 e linha 2.
        for (int i = 0; i + 2 < lines.Count; i += 3)
        {
            string name = lines[i].Trim();
            string line1 = lines[i + 1];
            string line2 = lines[i + 2];

            try
            {
                result.Add(ParseSingle(name, line1, line2, capturedAt));
            }
            catch (TleParsingException)
            {
                // Ignora registro malformado e segue para o proximo.
            }
        }

        return result;
    }

    /// <summary>Interpreta um unico objeto a partir de suas 3 linhas.</summary>
    public Debris ParseSingle(string name, string line1, string line2, DateTime capturedAt)
    {
        if (line1.Length < 7 || !line1.StartsWith('1'))
        {
            throw new TleParsingException("Linha 1 do TLE invalida.", line1);
        }

        if (line2.Length < 63 || !line2.StartsWith('2'))
        {
            throw new TleParsingException("Linha 2 do TLE invalida.", line2);
        }

        try
        {
            int noradId = int.Parse(line1.Substring(2, 5).Trim(), CultureInfo.InvariantCulture);

            double inclination = ParseDouble(line2.Substring(8, 8));
            double raan = ParseDouble(line2.Substring(17, 8));

            // Excentricidade vem sem o "0." inicial (ponto decimal implicito).
            string eccentricityDigits = line2.Substring(26, 7).Trim();
            double eccentricity = ParseDouble("0." + eccentricityDigits);

            double meanMotion = ParseDouble(line2.Substring(52, 11));

            var elements = new OrbitalElements(inclination, eccentricity, meanMotion, raan);
            return Debris.Create(noradId, name, line1, line2, elements, capturedAt);
        }
        catch (TleParsingException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new TleParsingException("Falha ao interpretar os campos numericos do TLE.", ex, line2);
        }
    }

    private static double ParseDouble(string value)
        => double.Parse(value.Trim(), CultureInfo.InvariantCulture);
}
