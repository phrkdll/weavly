using NSubstitute;
using Wolverine;

namespace Weavly.Core.Tests;

public abstract class WeavlyEndpointTests
{
    protected readonly IMessageBus messageBusMock = Substitute.For<IMessageBus>();
}
