using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Shouldly;
using Weavly.Core.Shared.Contracts;
using Weavly.Core.Shared.Implementation;
using Weavly.Core.Shared.Implementation.Endpoints;
using Weavly.Core.Tests.Shared;

namespace Weavly.Auth.Tests;

public abstract class AuthEndpointTests<TEndpoint, TRequest, TResponse>(TRequest request) : WeavlyEndpointTests
    where TEndpoint : EndpointBase<TRequest, TResponse>
    where TResponse : class
    where TRequest : IWeavlyCommand
{
    [Fact]
    public async Task HandleAsync_CallsInvokeAsync_OnMessageBus_AndReturnsOkOnSuccess()
    {
        MessageBusMock.InvokeAsync<Result>(Arg.Any<TRequest>()).Returns(Result.Success());

        var sut = Activator.CreateInstance(typeof(TEndpoint), MessageBusMock) as TEndpoint;

        var response = await sut!.HandleAsync(request, CancellationToken.None);
        response.ShouldBeOfType<Ok<Result>>();

        await MessageBusMock.Received().InvokeAsync<Result>(Arg.Any<TRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_CallsInvokeAsync_OnMessageBus_AndReturnsBadRequestOnError()
    {
        MessageBusMock.InvokeAsync<Result>(Arg.Any<TRequest>()).Returns(Result.Failure("Error"));

        var sut = Activator.CreateInstance(typeof(TEndpoint), MessageBusMock) as TEndpoint;

        var response = await sut!.HandleAsync(request, CancellationToken.None);
        response.ShouldBeOfType<BadRequest<Result>>();

        await MessageBusMock.Received().InvokeAsync<Result>(Arg.Any<TRequest>(), Arg.Any<CancellationToken>());
    }
}
