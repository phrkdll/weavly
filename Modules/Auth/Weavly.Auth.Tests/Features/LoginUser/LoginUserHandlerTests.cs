using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Shouldly;
using Weavly.Auth.Features.LoginUser;
using Weavly.Auth.Implementation;
using Weavly.Auth.Models;
using Weavly.Auth.Shared.Features.LoginUser;
using Weavly.Core.Implementation;
using Weavly.Core.Shared.Implementation;

namespace Weavly.Auth.Tests.Features.LoginUser;

public sealed class LoginUserHandlerTests : AuthHandlerTests
{
    private readonly LoginUserHandler sut;

    private readonly IPasswordHasher<AppUser> passwordHasherMock = Substitute.For<IPasswordHasher<AppUser>>();

    public LoginUserHandlerTests()
    {
        dbContextMock.Users.AddRange(
            AppUser.Create("admin@test.local", []),
            AppUser.Create("pending@test.local", [AppUserToken.CreateEmailValidationToken()]),
            AppUser.Create("2fa@test.local", [AppUserToken.CreateTwoFactorAuthenticationToken()])
        );

        dbContextMock.SaveChanges();

        sut = new LoginUserHandler(
            dbContextMock,
            passwordHasherMock,
            new JwtProvider(messageBusMock),
            new DefaultTimeProvider()
        );
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_SuccessInstance_WhenLoginWasValid()
    {
        passwordHasherMock
            .VerifyHashedPassword(Arg.Any<AppUser>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(PasswordVerificationResult.Success);

        var command = new LoginUserCommand("admin@test.local", "P@ssw0rd!");
        var result = await sut.HandleAsync(command, CancellationToken.None);
        var data = result.ShouldBeOfType<Success<LoginUserResponse>>().Data;

        data.ShouldNotBeNull();
        data.Token.ShouldNotBeNullOrWhiteSpace();
        data.ExpiresAt.ShouldBeGreaterThan(DateTime.UtcNow);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_FailureInstance_WhenUserNotFound()
    {
        passwordHasherMock
            .VerifyHashedPassword(Arg.Any<AppUser>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(PasswordVerificationResult.Success);

        var command = new LoginUserCommand("missing@test.local", "P@ssw0rd!");
        var result = await sut.HandleAsync(command, CancellationToken.None);
        result.ShouldBeOfType<Failure>();

        result.Message.ShouldBe("Email or password is incorrect.");
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_FailureInstance_WhenValidationIsPending()
    {
        passwordHasherMock
            .VerifyHashedPassword(Arg.Any<AppUser>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(PasswordVerificationResult.Success);

        var command = new LoginUserCommand("pending@test.local", "P@ssw0rd!");
        var result = await sut.HandleAsync(command, CancellationToken.None);
        result.ShouldBeOfType<Failure>();

        result.Message.ShouldBe("Email verification pending.");
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_FailureInstance_WhenPasswordIsWrong()
    {
        passwordHasherMock
            .VerifyHashedPassword(Arg.Any<AppUser>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(PasswordVerificationResult.Failed);

        var command = new LoginUserCommand("admin@test.local", "P@ssw0rd!");
        var result = await sut.HandleAsync(command, CancellationToken.None);
        result.ShouldBeOfType<Failure>();

        result.Message.ShouldBe("Email or password is incorrect.");
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_FailureInstance_WhenTwoFactorAuthIsEnabled()
    {
        passwordHasherMock
            .VerifyHashedPassword(Arg.Any<AppUser>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(PasswordVerificationResult.Success);

        var command = new LoginUserCommand("2fa@test.local", "P@ssw0rd!");
        var result = await sut.HandleAsync(command, CancellationToken.None);
        var data = result.ShouldBeOfType<Success<LoginUserResponse>>().Data;

        data.ShouldBe(LoginUserResponse.Empty);
    }
}
