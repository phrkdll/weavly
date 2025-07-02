using Weavly.Core.Shared.Contracts;

namespace Weavly.Core.Implementation;

internal sealed class DefaultTimeProvider : ITimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
