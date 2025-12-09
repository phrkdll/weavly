using NSubstitute;
using Weavly.Auth.Features.CreateAppUser;
using Weavly.Auth.Shared.Features.CreateAppUser;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Shared.Implementation;
using Wolverine;

namespace Weavly.Auth.Tests.Features.CreateAppUser;

public sealed class CreateAppUserEndpointTests
{
    private readonly IMessageBus busMock = Substitute.For<IMessageBus>();

    [Fact]
    public async Task CreateAppUserEndpoint_ShouldReturn_SuccessInstance_WhenNewUserWasCreated()
    {
        busMock.InvokeAsync<Result>(Arg.Any<CreateAppUserCommand>()).Returns(Success.Create(new AppUserId()));

        var sut = new CreateAppUserEndpoint(busMock);

        var request = new CreateAppUserCommand("admin@test.local", "Admin", "Admin", "P@ssw0rd!");
        await sut.HandleAsync(request, CancellationToken.None);

        await busMock.Received().InvokeAsync<Result>(Arg.Any<CreateAppUserCommand>(), Arg.Any<CancellationToken>());
    }
}
