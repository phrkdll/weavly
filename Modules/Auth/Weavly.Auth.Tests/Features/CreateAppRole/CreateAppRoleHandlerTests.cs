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
    private readonly CreateAppRoleHandler sut;

    public CreateAppRoleHandlerTests()
    {
        sut = new CreateAppRoleHandler(Repository, Substitute.For<ILogger<CreateAppRoleHandler>>());
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_SuccessInstance_WhenNewRoleWasCreated()
    {
        var command = new CreateAppRoleCommand("TestRole");
        var result = await sut.HandleAsync(command, CancellationToken.None);
        var data = result.ShouldBeOfType<Success<CreateAppRoleResponse>>().Data;

        data.Id.Value.ShouldNotBeNull();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_FailureInstance_WhenRoleAlreadyExists()
    {
        var existingRole = AppRole.Create("ExistingRole");
        await Repository.Roles.InsertAsync(existingRole);

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
