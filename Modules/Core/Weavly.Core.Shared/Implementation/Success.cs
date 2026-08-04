namespace Weavly.Core.Shared.Implementation;

public record Success<T>(T Data, string? Message) : Result(true, Message)
{
}