using System.Diagnostics.CodeAnalysis;

namespace Weavly.Core.Shared.Implementation.Results;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class Result
{
    protected Result() { }

    protected Result(bool success, string? message)
    {
        Success = success;
        Message = message;
    }

    public bool Success { get; init; }

    public string? Message { get; init; }
}
