using NSubstitute;
using Weavly.Auth.Features.CreateAppRole;
using Weavly.Auth.Shared.Features.CreateAppRole;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Shared.Implementation;
using Wolverine;

namespace Weavly.Auth.Tests.Features.CreateAppRole;

public sealed class CreateAppRoleEndpointTests
{
    private readonly IMessageBus busMock = Substitute.For<IMessageBus>();

    [Fact]
    public async Task HandleAsync_CallsInvokeAsync_OnMessageBus()
    {
        busMock.InvokeAsync<Result>(Arg.Any<CreateAppRoleCommand>()).Returns(Success.Create(new AppRoleId()));

        var sut = new CreateAppRoleEndpoint(busMock);

        var request = new CreateAppRoleCommand("Test");
        await sut.HandleAsync(request, CancellationToken.None);

        await busMock.Received().InvokeAsync<Result>(Arg.Any<CreateAppRoleCommand>(), Arg.Any<CancellationToken>());
    }
}
