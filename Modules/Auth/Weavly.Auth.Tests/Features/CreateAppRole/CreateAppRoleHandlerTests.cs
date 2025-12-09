using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Weavly.Auth.Features.CreateAppRole;
using Weavly.Auth.Models;
using Weavly.Auth.Shared.Features.CreateAppRole;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Shared.Implementation;

namespace Weavly.Auth.Tests.Features.CreateAppRole;

public sealed class CreateAppRoleHandlerTests : AuthHandlerTests
{
    private readonly ILogger<CreateAppRoleHandler> loggerMock = Substitute.For<ILogger<CreateAppRoleHandler>>();

    private readonly CreateAppRoleHandler sut;

    public CreateAppRoleHandlerTests()
    {
        sut = new CreateAppRoleHandler(dbContextMock, loggerMock);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_SuccessInstance_WhenNewRoleWasCreated()
    {
        var command = new CreateAppRoleCommand("TestRole");
        var result = await sut.HandleAsync(command, CancellationToken.None);
        var data = result.ShouldBeOfType<Success<AppRoleId>>().Data.ShouldBeOfType<AppRoleId>();

        data.Value.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_FailureInstance_WhenRoleAlreadyExists()
    {
        var existingRole = AppRole.Create("ExistingRole");
        dbContextMock.Roles.AddRange(existingRole);
        await dbContextMock.SaveChangesAsync();

        var command = new CreateAppRoleCommand("ExistingRole");
        var result = await sut.HandleAsync(command, CancellationToken.None);

        result.ShouldBeOfType<Failure>();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_FailureInstance_WhenExceptionWasThrown()
    {
        var command = new CreateAppRoleCommand("");
        var result = await sut.HandleAsync(command, CancellationToken.None);

        result.ShouldBeOfType<Failure>();
    }
}
