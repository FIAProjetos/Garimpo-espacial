using Garimpo.Domain.Enums;

namespace Garimpo.Domain.Entities.SpaceAssets;

/// <summary>
/// Classe abstrata base para ativos espaciais rastreados pela plataforma.
/// Demonstra heranca e polimorfismo exigidos pela disciplina de Arquitetura:
/// Satelites, Sensores e Detritos compartilham comportamento comum via esta hierarquia.
/// </summary>
public abstract class SpaceAsset
{
    public Guid Id { get; protected set; }
    public string Name { get; protected set; } = string.Empty;
    public DateTime RegisteredAt { get; protected set; }

    public abstract AssetCategory Category { get; }

    /// <summary>Resumo polimorfico do ativo para dashboards e logs de auditoria.</summary>
    public abstract string GetSummary();

    /// <summary>Identificador unico de rastreamento (NORAD, sensor ID, etc.).</summary>
    public abstract string GetTrackingId();
}
