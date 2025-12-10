using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Shouldly;
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
    public async Task HandleAsync_CallsInvokeAsync_OnMessageBus_AndReturnsOkOnSuccess()
    {
        messageBusMock.InvokeAsync<Result>(Arg.Any<TRequest>()).Returns(Success.Create());

        var sut = Activator.CreateInstance(typeof(TEndpoint), messageBusMock) as TEndpoint;

        var response = await sut!.HandleAsync(request, CancellationToken.None);
        response.ShouldBeOfType<Ok<Result>>();

        await messageBusMock.Received().InvokeAsync<Result>(Arg.Any<TRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_CallsInvokeAsync_OnMessageBus_AndReturnsBadRequestOnError()
    {
        messageBusMock.InvokeAsync<Result>(Arg.Any<TRequest>()).Returns(Failure.Create("Error"));

        var sut = Activator.CreateInstance(typeof(TEndpoint), messageBusMock) as TEndpoint;

        var response = await sut!.HandleAsync(request, CancellationToken.None);
        response.ShouldBeOfType<BadRequest<Result>>();

        await messageBusMock.Received().InvokeAsync<Result>(Arg.Any<TRequest>(), Arg.Any<CancellationToken>());
    }
}
