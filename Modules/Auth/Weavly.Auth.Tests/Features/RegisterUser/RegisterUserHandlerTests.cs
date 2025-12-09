using EntityFrameworkCore.Testing.NSubstitute;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using Weavly.Auth.Enums;
using Weavly.Auth.Features.RegisterUser;
using Weavly.Auth.Models;
using Weavly.Auth.Persistence;
using Weavly.Auth.Shared.Events;
using Weavly.Auth.Shared.Features.RegisterUser;
using Weavly.Core.Shared.Implementation;
using Weavly.Mail.Shared.Features.SendMail;
using Wolverine;

namespace Weavly.Auth.Tests.Features.RegisterUser;

public sealed class RegisterUserHandlerTests
{
    private readonly IMessageBus busMock = Substitute.For<IMessageBus>();

    private readonly AuthDbContext dbContextMock;

    private readonly RegisterUserHandler sut;

    public RegisterUserHandlerTests()
    {
        var dbContextOptions = new DbContextOptionsBuilder()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .UseStronglyTypeConverters();

        dbContextMock = Create.MockedDbContextFor<TestAuthDbContext>(dbContextOptions.Options);

        sut = new RegisterUserHandler(dbContextMock, new PasswordHasher<AppUser>(), busMock);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_SuccessInstance_WhenNewUserWasCreated()
    {
        var command = new RegisterUserCommand("admin@test.local", "P@ssw0rd!");
        var result = await sut.HandleAsync(command, CancellationToken.None);
        var data = result.ShouldBeOfType<Success<RegisterUserResponse>>().Data;

        data.ShouldNotBeNull();
        data.Id.ShouldNotBeNull();

        var user = await dbContextMock.Users.FirstOrDefaultAsync();
        user.ShouldNotBeNull();
        user.Tokens.ShouldContain(t => t.Purpose == AppUserTokenPurpose.EmailValidation);
        user.PasswordHash.ShouldNotBeNullOrEmpty();
        user.PasswordHash.ShouldNotBe(command.Password);

        await busMock.Received().PublishAsync(Arg.Any<SendMailCommand>());
        await busMock.Received().PublishAsync(Arg.Any<AppUserRegisteredEvent>());
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_FailureInstance_WhenUserAlreadyExists()
    {
        var user = AppUser.Create("admin@test.local", [AppUserToken.CreateEmailValidationToken()]);
        dbContextMock.Users.AddRange(user);
        await dbContextMock.SaveChangesAsync();

        var command = new RegisterUserCommand("admin@test.local", "P@ssw0rd!");
        var result = await sut.HandleAsync(command, CancellationToken.None);

        result.ShouldBeOfType<Failure>();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_FailureInstance_WhenExceptionWasThrown()
    {
        dbContextMock.Users.Throws(new Exception("Database error"));

        var command = new RegisterUserCommand("admin@test.local", "P@ssw0rd!");
        var result = await sut.HandleAsync(command, CancellationToken.None);

        result.ShouldBeOfType<Failure>();
    }
}
