using System.Text.Json.Serialization;

namespace Weavly.Core.Shared.Implementation.Results;

public class Failure : Result
{
    [JsonConstructor]
    private Failure() { }

    private Failure(string message)
        : base(false, message) { }

    public static Failure Create(string message) => new(message);

    public static Failure Create(Exception exception) => new(exception.Message);
}
