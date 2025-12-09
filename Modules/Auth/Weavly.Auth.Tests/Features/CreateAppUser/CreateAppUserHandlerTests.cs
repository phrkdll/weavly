using EntityFrameworkCore.Testing.NSubstitute;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Weavly.Auth.Features.CreateAppUser;
using Weavly.Auth.Models;
using Weavly.Auth.Persistence;
using Weavly.Auth.Shared.Features.CreateAppUser;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Shared.Implementation;

namespace Weavly.Auth.Tests.Features.CreateAppUser;

public sealed class CreateAppUserHandlerTests
{
    private readonly ILogger<CreateAppUserHandler> loggerMock = Substitute.For<ILogger<CreateAppUserHandler>>();

    private readonly AuthDbContext dbContextMock;

    private readonly CreateAppUserHandler sut;

    public CreateAppUserHandlerTests()
    {
        var dbContextOptions = new DbContextOptionsBuilder()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .UseStronglyTypeConverters();

        dbContextMock = Create.MockedDbContextFor<TestAuthDbContext>(dbContextOptions.Options);

        sut = new CreateAppUserHandler(dbContextMock, loggerMock);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_SuccessInstance_WhenNewRoleWasCreated()
    {
        var command = new CreateAppUserCommand("admin@test.local", "Admin", "Admin", "P@ssw0rd!");
        var result = await sut.HandleAsync(command, CancellationToken.None);
        var data = result.ShouldBeOfType<Success<AppUserId>>().Data.ShouldBeOfType<AppUserId>();

        data.Value.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_FailureInstance_WhenRoleAlreadyExists()
    {
        var user = AppUser.Create("admin@test.local", [AppUserToken.CreateEmailValidationToken()]);
        dbContextMock.Users.AddRange(user);
        await dbContextMock.SaveChangesAsync();

        var command = new CreateAppUserCommand("admin@test.local", "Admin", "Admin");
        var result = await sut.HandleAsync(command, CancellationToken.None);

        result.ShouldBeOfType<Failure>();
    }
}
