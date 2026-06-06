using Garimpo.Domain.Entities;

namespace Garimpo.Application.Ports;

/// <summary>
/// Porta de saida (driven port) para a fonte externa de dados TLE (ex.: Celestrak/NORAD).
/// O adapter e responsavel por buscar o arquivo TLE de forma assincrona e converte-lo
/// em entidades de dominio <see cref="Debris"/>.
/// </summary>
public interface ITleProvider
{
    /// <summary>
    /// Busca e interpreta o catalogo TLE de um grupo da Celestrak.
    /// </summary>
    /// <param name="group">Grupo Celestrak (ex.: "cosmos-2251-debris", "active"). Se nulo, usa o padrao.</param>
    Task<IReadOnlyList<Debris>> FetchDebrisAsync(string? group = null, CancellationToken cancellationToken = default);
}
