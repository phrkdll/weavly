using NSubstitute;
using Shouldly;
using Weavly.Auth.Features.EnableTwoFactorAuth;
using Weavly.Auth.Models;
using Weavly.Auth.Shared.Features.EnableTwoFactorAuth;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Shared.Contracts;
using Weavly.Core.Shared.Implementation;

namespace Weavly.Auth.Tests.Features.EnableTwoFactorAuth;

public sealed class EnableTwoFactorAuthHandlerTests : AuthHandlerTests
{
    private readonly EnableTwoFactorAuthHandler sut;
    private readonly IUserContext<AppUserId> userContextMock = Substitute.For<IUserContext<AppUserId>>();

    public EnableTwoFactorAuthHandlerTests()
    {
        this.sut = new EnableTwoFactorAuthHandler(this.Repository, this.userContextMock);
    }

    [Fact]
    public async Task HandleAsync_ReturnsFailure_WhenUserNotFound()
    {
        this.userContextMock.UserId.Returns(new AppUserId());
        var result = await this.sut.HandleAsync(new EnableTwoFactorAuthCommand());

        result.ShouldBeOfType<Failure>();
        result.Message.ShouldBe("User not found");
    }

    [Fact]
    public async Task HandleAsync_ReturnsFailure_WhenTwoFactorAuth_IsAlreadyEnabled()
    {
        var user = AppUser.Create("admin@test.local", [AppUserToken.CreateTwoFactorAuthenticationToken()]);
        await this.Repository.Users.InsertAsync(user);

        this.userContextMock.UserId.Returns(user.Id);

        var result = await this.sut.HandleAsync(new EnableTwoFactorAuthCommand());

        result.ShouldBeOfType<Failure>();
        result.Message.ShouldBe("2FA is already enabled");
    }

    [Fact]
    public async Task HandleAsync_ReturnsSuccess_WhenUserExists_AndTwoFactorAuth_IsNotEnabled()
    {
        var user = AppUser.Create("admin@test.local", []);
        await this.Repository.Users.InsertAsync(user);

        this.userContextMock.UserId.Returns(user.Id);

        var result = await this.sut.HandleAsync(new EnableTwoFactorAuthCommand());

        var data = result.ShouldBeOfType<Success<EnableTwoFactorAuthResponse>>().Data;
        data.QrCode.ShouldNotBeEmpty();
        data.TextCode.ShouldNotBeEmpty();
    }
}