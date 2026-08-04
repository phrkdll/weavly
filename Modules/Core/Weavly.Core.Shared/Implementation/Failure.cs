namespace Weavly.Core.Shared.Implementation;

public record Failure(string Message, Exception? Exception = null) : Result(false, Message);