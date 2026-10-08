using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Weavly.Auth.Features.SeedAppRole;
using Weavly.Auth.Features.SeedAppUser;
using Weavly.Auth.Models;
using Weavly.Auth.Shared.Features.SeedAppRole;
using Weavly.Auth.Shared.Features.SeedAppUser;
using Weavly.Core.Shared.Implementation;

namespace Weavly.Auth.Tests.Features.Seeding;

public sealed class SeedHandlersTests : AuthHandlerTests
{
    [Fact]
    public async Task SeedAppRole_ShouldSucceedWithoutChangingExistingRole()
    {
        var existingRole = AppRole.Create("Default");
        await Repository.Roles.InsertAsync(existingRole);
        var handler = new SeedAppRoleHandler(Repository, Substitute.For<ILogger<SeedAppRoleHandler>>());

        var result = await handler.HandleAsync(new SeedAppRoleCommand("Default"));

        var response = result.ShouldBeOfType<Success<SeedAppRoleResponse>>().Data;
        response.Created.ShouldBeFalse();
        response.Id.ShouldBe(existingRole.Id);
        (await Repository.Roles.FilterAsync(x => x.Name == "Default")).Count().ShouldBe(1);
    }

    [Fact]
    public async Task SeedAppUser_ShouldRequireRole_AndBeIdempotent()
    {
        var userHandler = new SeedAppUserHandler(Repository, Substitute.For<ILogger<SeedAppUserHandler>>());
        var missingRole = await userHandler.HandleAsync(new SeedAppUserCommand("seed@test.local", "seed", "System"));
        missingRole.ShouldBeOfType<Failure>();

        var role = AppRole.Create("System");
        await Repository.Roles.InsertAsync(role);

        var command = new SeedAppUserCommand("seed@test.local", "seed", "System");
        var created = (await userHandler.HandleAsync(command)).ShouldBeOfType<Success<SeedAppUserResponse>>().Data;
        var repeated = (await userHandler.HandleAsync(command)).ShouldBeOfType<Success<SeedAppUserResponse>>().Data;

        created.Created.ShouldBeTrue();
        repeated.Created.ShouldBeFalse();
        repeated.Id.ShouldBe(created.Id);
        (await Repository.Users.FilterAsync(x => x.Email == command.Email)).Count().ShouldBe(1);
    }
}
