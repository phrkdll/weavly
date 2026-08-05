using Weavly.Core.Shared.Models;

namespace Weavly.Core.Shared.Implementation;

public record Failure(string Message, Exception? Exception = null) : Result(false, Message);
