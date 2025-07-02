using Weavly.Core.Implementation;
using Shouldly;

namespace Weavly.Core.Tests.Implementation;

public class DefaultTimeProviderTests
{
    [Fact]
    public void DefaultTimeProvider_ReturnsUtcTime()
    {
        var timeProvider = new DefaultTimeProvider();

        var providerTime = timeProvider.UtcNow;

        providerTime.ShouldBeOfType<DateTime>();
    }
}
