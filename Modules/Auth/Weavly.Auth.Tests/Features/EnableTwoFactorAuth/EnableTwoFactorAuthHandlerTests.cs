using NSubstitute;
using Shouldly;
using Weavly.Auth.Features.EnableTwoFactorAuth;
using Weavly.Auth.Models;
using Weavly.Auth.Shared.Features.EnableTwoFactorAuth;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Shared.Implementation;

namespace Weavly.Auth.Tests.Features.EnableTwoFactorAuth;

public sealed class EnableTwoFactorAuthHandlerTests : AuthHandlerTests
{
    private readonly EnableTwoFactorAuthHandler sut;

    public EnableTwoFactorAuthHandlerTests()
    {
        sut = new EnableTwoFactorAuthHandler(dbContextMock, userContextMock);
    }

    [Fact]
    public async Task HandleAsync_ReturnsFailure_WhenUserNotFound()
    {
        userContextMock.UserId.Returns(new AppUserId());
        var result = await sut.HandleAsync(new EnableTwoFactorAuthCommand());

        result.ShouldBeOfType<Failure>();
        result.Message.ShouldBe("User not found");
    }

    [Fact]
    public async Task HandleAsync_ReturnsFailure_WhenTwoFactorAuth_IsAlreadyEnabled()
    {
        var user = AppUser.Create("admin@test.local", [AppUserToken.CreateTwoFactorAuthenticationToken()]);
        dbContextMock.Users.Add(user);
        await dbContextMock.SaveChangesAsync();

        userContextMock.UserId.Returns(user.Id);

        var result = await sut.HandleAsync(new EnableTwoFactorAuthCommand());

        result.ShouldBeOfType<Failure>();
        result.Message.ShouldBe("2FA is already enabled");
    }

    [Fact]
    public async Task HandleAsync_ReturnsSuccess_WhenUserExists_AndTwoFactorAuth_IsNotEnabled()
    {
        var user = AppUser.Create("admin@test.local", []);
        dbContextMock.Users.Add(user);
        await dbContextMock.SaveChangesAsync();

        userContextMock.UserId.Returns(user.Id);

        var result = await sut.HandleAsync(new EnableTwoFactorAuthCommand());

        var data = result.ShouldBeOfType<Success<EnableTwoFactorAuthResponse>>().Data;
        data.QrCode.ShouldNotBeEmpty();
        data.TextCode.ShouldNotBeEmpty();
    }
}
