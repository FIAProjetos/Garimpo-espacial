namespace Garimpo.Domain.Exceptions;

/// <summary>
/// Lancada quando uma entrada TLE (Two-Line Element) esta malformada ou nao pode ser
/// interpretada. Permite que a ingestao ignore registros invalidos sem derrubar o pipeline.
/// </summary>
public sealed class TleParsingException : DomainException
{
    public string? RawContent { get; }

    public TleParsingException(string message, string? rawContent = null)
        : base(message)
    {
        RawContent = rawContent;
    }

    public TleParsingException(string message, Exception innerException, string? rawContent = null)
        : base(message, innerException)
    {
        RawContent = rawContent;
    }
}
