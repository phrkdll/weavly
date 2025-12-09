using Weavly.Core.Shared.Contracts;

namespace Weavly.Core.Implementation;

public sealed class DefaultTimeProvider : ITimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
