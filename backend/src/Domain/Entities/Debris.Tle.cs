namespace Garimpo.Domain.Entities;

/// <summary>
/// Parte parcial de <see cref="Debris"/> com operacoes sobre o TLE bruto.
/// Separa a logica de inspecao/validacao da entidade principal (organizacao de codigo).
/// </summary>
public partial class Debris
{
    /// <summary>Verifica se as linhas TLE possuem formato minimo valido.</summary>
    public bool HasValidTleFormat()
        => Line1.Length >= 7 && Line1.StartsWith('1')
           && Line2.Length >= 63 && Line2.StartsWith('2');

    /// <summary>Retorna hash das linhas TLE para detectar alteracoes (integridade).</summary>
    public string ComputeTleIntegrityHash()
    {
        string combined = $"{Line1}|{Line2}";
        byte[] hash = System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(combined));
        return Convert.ToHexString(hash)[..16];
    }
}
