using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Weavly.Auth.Features.CreateAppUser;
using Weavly.Auth.Models;
using Weavly.Auth.Shared.Features.CreateAppUser;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Shared.Implementation;

namespace Weavly.Auth.Tests.Features.CreateAppUser;

public sealed class CreateAppUserHandlerTests : AuthHandlerTests
{
    private readonly ILogger<CreateAppUserHandler> loggerMock = Substitute.For<ILogger<CreateAppUserHandler>>();

    private readonly CreateAppUserHandler sut;

    public CreateAppUserHandlerTests()
    {
        this.sut = new CreateAppUserHandler(this.Repository, this.loggerMock);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_SuccessInstance_WhenNewRoleWasCreated()
    {
        var command = new CreateAppUserCommand("admin@test.local", "Admin", "Admin", "P@ssw0rd!");
        var result = await this.sut.HandleAsync(command, CancellationToken.None);
        var data = result.ShouldBeOfType<Success<AppUserId>>().Data.ShouldBeOfType<AppUserId>();

        data.Value.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_FailureInstance_WhenRoleAlreadyExists()
    {
        var user = AppUser.Create("admin@test.local", [AppUserToken.CreateEmailValidationToken()]);
        await this.Repository.Users.InsertAsync(user);

        var command = new CreateAppUserCommand("admin@test.local", "Admin", "Admin");
        var result = await this.sut.HandleAsync(command, CancellationToken.None);

        result.ShouldBeOfType<Failure>();
    }
}
