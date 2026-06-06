namespace Garimpo.Domain.Enums;

/// <summary>
/// Categoriza um detrito orbital de acordo com a faixa de altitude em que se encontra.
/// O foco do Garimpo Espacial e a Orbita Baixa da Terra (LEO), onde a densidade de lixo
/// espacial e critica para a Sindrome de Kessler.
/// </summary>
public enum DebrisClassification
{
    /// <summary>Altitude abaixo de 2.000 km (Low Earth Orbit).</summary>
    LowEarthOrbit = 0,

    /// <summary>Altitude entre 2.000 km e 35.786 km (Medium Earth Orbit).</summary>
    MediumEarthOrbit = 1,

    /// <summary>Altitude proxima a 35.786 km (Geostationary Earth Orbit).</summary>
    GeostationaryOrbit = 2,

    /// <summary>Altitude acima da faixa geoestacionaria.</summary>
    HighEarthOrbit = 3
}
