using NSubstitute;
using Wolverine;

namespace Weavly.Core.Tests.Shared;

public abstract class WeavlyEndpointTests
{
    protected readonly IMessageBus MessageBusMock = Substitute.For<IMessageBus>();
}
