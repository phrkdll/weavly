using Microsoft.EntityFrameworkCore;
using Shouldly;
using Weavly.Auth.Features.TokenLogin;
using Weavly.Auth.Implementation;
using Weavly.Auth.Models;
using Weavly.Auth.Shared.Features.TokenLogin;
using Weavly.Core.Shared.Implementation;

namespace Weavly.Auth.Tests.Features.TokenLogin;

public sealed class TokenLoginHandlerTests : AuthHandlerTests
{
    private readonly TokenLoginHandler sut;

    public TokenLoginHandlerTests()
    {
        sut = new TokenLoginHandler(dbContextMock, new JwtProvider(messageBusMock));
    }

    [Fact]
    public async Task HandleAsync_ReturnsFailure_WhenUserNotFound()
    {
        var result = await sut.HandleAsync(new TokenLoginCommand(Guid.NewGuid()));

        result.ShouldBeOfType<Failure>();
        result.Message.ShouldBe("Invalid login token.");
    }

    [Fact]
    public async Task HandleAsync_ReturnsFailure_WhenTokenIsInvalid()
    {
        dbContextMock.Users.Add(
            AppUser.Create("admin@test.local", [AppUserToken.CreateTwoFactorAuthenticationToken()])
        );
        await dbContextMock.SaveChangesAsync();

        var result = await sut.HandleAsync(new TokenLoginCommand(Guid.NewGuid()));

        result.ShouldBeOfType<Failure>();
        result.Message.ShouldBe("Invalid login token.");
    }

    [Fact]
    public async Task HandleAsync_ReturnsSuccess_WhenTokenIsValid()
    {
        var token = AppUserToken.CreateTwoFactorAuthenticationToken();
        dbContextMock.Users.Add(AppUser.Create("admin@test.local", [token]));
        await dbContextMock.SaveChangesAsync();

        var result = await sut.HandleAsync(new TokenLoginCommand(token.Value));

        var data = result.ShouldBeOfType<Success<TokenLoginResponse>>().Data;
        data.ExpiresAt.ShouldBeGreaterThan(DateTime.UtcNow);
        data.Token.ShouldNotBeNullOrWhiteSpace();

        var user = await dbContextMock.Users.Include(x => x.Tokens).SingleAsync();
        user.Tokens.ShouldNotContain(token);
    }
}
