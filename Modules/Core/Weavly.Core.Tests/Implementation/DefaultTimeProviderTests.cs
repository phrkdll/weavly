using Shouldly;
using Weavly.Core.Implementation;

namespace Weavly.Core.Tests.Implementation;

public sealed class DefaultTimeProviderTests
{
    [Fact]
    public void DefaultTimeProvider_ReturnsUtcTime()
    {
        var timeProvider = new DefaultTimeProvider();

        var providerTime = timeProvider.UtcNow;

        providerTime.ShouldBeOfType<DateTime>();
    }
}
