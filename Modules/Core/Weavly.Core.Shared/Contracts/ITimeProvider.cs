namespace Weavly.Core.Shared.Contracts;

public interface ITimeProvider
{
    DateTime UtcNow { get; }
}
