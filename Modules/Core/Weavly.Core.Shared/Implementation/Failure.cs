namespace Weavly.Core.Shared.Implementation;

public record Failure(string Message, Failure? InnerFailure, Exception? Exception = null) : Result(false, Message);
