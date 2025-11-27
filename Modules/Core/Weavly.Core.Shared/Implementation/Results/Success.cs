using System.Text.Json.Serialization;

namespace Weavly.Core.Shared.Implementation.Results;

public class Success<T> : Result
{
    [JsonConstructor]
    private Success() { }

    private Success(T data, string? message)
        : base(true, message)
    {
        Data = data;
    }

    public T Data { get; init; } = default!;

    public static Success<T> Create(T data, string? message = null) => new(data, message);
}

public static class Success
{
    public static Success<T> Create<T>(T data, string? message = null) => Success<T>.Create(data, message);
}
