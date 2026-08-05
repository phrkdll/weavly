using NSubstitute;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Core.Tests.Shared;

public abstract class WeavlyHandlerTests : WeavlyEndpointTests
{
    protected readonly ITimeProvider TimeProviderMock = Substitute.For<ITimeProvider>();
}
