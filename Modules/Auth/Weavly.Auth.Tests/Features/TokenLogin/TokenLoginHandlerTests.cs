using Shouldly;
using Weavly.Auth.Features.TokenLogin;
using Weavly.Auth.Implementation.JsonWebToken;
using Weavly.Auth.Models;
using Weavly.Auth.Shared.Features.TokenLogin;
using Weavly.Core.Shared.Implementation;

namespace Weavly.Auth.Tests.Features.TokenLogin;

public sealed class TokenLoginHandlerTests : AuthHandlerTests
{
    private readonly TokenLoginHandler sut;

    public TokenLoginHandlerTests()
    {
        this.sut = new TokenLoginHandler(this.Repository, new JwtProvider(this.MessageBusMock));
    }

    [Fact]
    public async Task HandleAsync_ReturnsFailure_WhenUserNotFound()
    {
        var result = await this.sut.HandleAsync(new TokenLoginCommand(Guid.NewGuid()));

        result.ShouldBeOfType<Failure>();
        result.Message.ShouldBe("Invalid login token.");
    }

    [Fact]
    public async Task HandleAsync_ReturnsFailure_WhenTokenIsInvalid()
    {
        await this.Repository.Users.InsertAsync(
            AppUser.Create("admin@test.local", [AppUserToken.CreateTwoFactorAuthenticationToken()])
        );

        var result = await this.sut.HandleAsync(new TokenLoginCommand(Guid.NewGuid()));

        result.ShouldBeOfType<Failure>();
        result.Message.ShouldBe("Invalid login token.");
    }

    [Fact]
    public async Task HandleAsync_ReturnsSuccess_WhenTokenIsValid()
    {
        var token = AppUserToken.CreateLoginToken();
        await this.Repository.Users.InsertAsync(AppUser.Create("admin@test.local", [token]));

        var result = await this.sut.HandleAsync(new TokenLoginCommand(token.Value));

        var data = result.ShouldBeOfType<Success<TokenLoginResponse>>().Data;
        data.ExpiresAt.ShouldBeGreaterThan(DateTime.UtcNow);
        data.Token.ShouldNotBeNullOrWhiteSpace();

        var user = await this.Repository.Users.FindAsync(x => x.Email == "admin@test.local");

        user.ShouldNotBeNull();
        user.Tokens.ShouldNotContain(token);
    }
}
