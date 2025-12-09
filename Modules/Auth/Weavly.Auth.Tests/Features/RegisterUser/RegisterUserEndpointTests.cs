using NSubstitute;
using Weavly.Auth.Features.RegisterUser;
using Weavly.Auth.Shared.Features.RegisterUser;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Shared.Implementation;
using Wolverine;

namespace Weavly.Auth.Tests.Features.RegisterUser;

public sealed class RegisterUserEndpointTests
{
    private readonly IMessageBus busMock = Substitute.For<IMessageBus>();

    [Fact]
    public async Task HandleAsync_CallsInvokeAsync_OnMessageBus()
    {
        busMock
            .InvokeAsync<Result>(Arg.Any<RegisterUserCommand>())
            .Returns(Success.Create(new RegisterUserResponse(new AppUserId())));

        var sut = new RegisterUserEndpoint(busMock);

        var request = new RegisterUserCommand("admin@test.local", "P@ssw0rd!");
        await sut.HandleAsync(request, CancellationToken.None);

        await busMock.Received().InvokeAsync<Result>(Arg.Any<RegisterUserCommand>(), Arg.Any<CancellationToken>());
    }
}
