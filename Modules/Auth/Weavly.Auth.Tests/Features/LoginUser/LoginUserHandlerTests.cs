using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Shouldly;
using Weavly.Auth.Features.LoginUser;
using Weavly.Auth.Implementation.JsonWebToken;
using Weavly.Auth.Models;
using Weavly.Auth.Shared.Features.LoginUser;
using Weavly.Core.Implementation;
using Weavly.Core.Shared.Implementation;

namespace Weavly.Auth.Tests.Features.LoginUser;

public sealed class LoginUserHandlerTests : AuthHandlerTests
{
    private readonly IPasswordHasher<AppUser> passwordHasherMock = Substitute.For<IPasswordHasher<AppUser>>();

    private readonly LoginUserHandler sut;

    public LoginUserHandlerTests()
    {
        this.Repository.Users.InsertAsync(AppUser.Create("admin@test.local", []));
        this.Repository.Users.InsertAsync(
            AppUser.Create("pending@test.local", [AppUserToken.CreateEmailValidationToken()])
        );
        this.Repository.Users.InsertAsync(
            AppUser.Create("2fa@test.local", [AppUserToken.CreateTwoFactorAuthenticationToken()])
        );

        this.passwordHasherMock.VerifyHashedPassword(Arg.Any<AppUser>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(PasswordVerificationResult.Success);

        this.sut = new LoginUserHandler(
            this.Repository,
            this.passwordHasherMock,
            new JwtProvider(this.MessageBusMock),
            new DefaultTimeProvider()
        );
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_SuccessInstance_WhenLoginWasValid()
    {
        var command = new LoginUserCommand("admin@test.local", "P@ssw0rd!");
        var result = await this.sut.HandleAsync(command, CancellationToken.None);
        var data = result.ShouldBeOfType<Success<LoginUserResponse>>().Data;

        data.ShouldNotBeNull();
        data.Token.ShouldNotBeNullOrWhiteSpace();
        data.ExpiresAt.ShouldBeGreaterThan(DateTime.UtcNow);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_FailureInstance_WhenUserNotFound()
    {
        var command = new LoginUserCommand("missing@test.local", "P@ssw0rd!");
        var result = await this.sut.HandleAsync(command, CancellationToken.None);
        result.ShouldBeOfType<Failure>();

        result.Message.ShouldBe("Email or password is incorrect.");
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_FailureInstance_WhenValidationIsPending()
    {
        var command = new LoginUserCommand("pending@test.local", "P@ssw0rd!");
        var result = await this.sut.HandleAsync(command, CancellationToken.None);
        result.ShouldBeOfType<Failure>();

        result.Message.ShouldBe("Email verification pending.");
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_FailureInstance_WhenPasswordIsWrong()
    {
        this.passwordHasherMock.VerifyHashedPassword(Arg.Any<AppUser>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(PasswordVerificationResult.Failed);

        var command = new LoginUserCommand("admin@test.local", "P@ssw0rd!");
        var result = await this.sut.HandleAsync(command, CancellationToken.None);
        result.ShouldBeOfType<Failure>();

        result.Message.ShouldBe("Email or password is incorrect.");
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_FailureInstance_WhenTwoFactorAuthIsEnabled()
    {
        var command = new LoginUserCommand("2fa@test.local", "P@ssw0rd!");
        var result = await this.sut.HandleAsync(command, CancellationToken.None);
        var data = result.ShouldBeOfType<Success<LoginUserResponse>>().Data;

        data.ShouldBe(LoginUserResponse.Empty);
    }
}
