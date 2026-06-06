namespace Garimpo.Application.Ports;

/// <summary>
/// Porta que confirma (commit) as alteracoes pendentes em uma unica transacao logica.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
