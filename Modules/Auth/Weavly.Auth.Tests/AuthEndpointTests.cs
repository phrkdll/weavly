using NSubstitute;
using Weavly.Core.Shared.Contracts;
using Weavly.Core.Shared.Implementation;
using Weavly.Core.Shared.Implementation.Endpoints;
using Weavly.Core.Tests;

namespace Weavly.Auth.Tests;

public abstract class AuthEndpointTests<TEndpoint, TRequest>(TRequest request) : WeavlyEndpointTests
    where TEndpoint : EndpointBase<TRequest>
    where TRequest : IWeavlyCommand
{
    [Fact]
    public async Task HandleAsync_CallsInvokeAsync_OnMessageBus()
    {
        messageBusMock.InvokeAsync<Result>(Arg.Any<TRequest>()).Returns(Success.Create());

        var sut = Activator.CreateInstance(typeof(TEndpoint), messageBusMock) as TEndpoint;

        await sut!.HandleAsync(request, CancellationToken.None);

        await messageBusMock.Received().InvokeAsync<Result>(Arg.Any<TRequest>(), Arg.Any<CancellationToken>());
    }
}
