using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using Weavly.Auth.Features.UserInfo;
using Weavly.Auth.Models;
using Weavly.Auth.Shared.Features.UserInfo;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Shared.Contracts;
using Weavly.Core.Shared.Implementation;

namespace Weavly.Auth.Tests.Features.UserInfo;

public sealed class UserInfoHandlerTests : AuthHandlerTests
{
    private readonly UserInfoHandler sut;
    private readonly IUserContext<AppUserId> userContextMock = Substitute.For<IUserContext<AppUserId>>();

    public UserInfoHandlerTests()
    {
        this.sut = new UserInfoHandler(this.Repository, this.userContextMock);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_SuccessInstance_ForExistingUser()
    {
        var user = AppUser.Create("admin@test.local", []);
        await this.Repository.Users.InsertAsync(user);

        this.userContextMock.UserId.Returns(user.Id);

        var result = await this.sut.HandleAsync(new UserInfoCommand(), CancellationToken.None);

        var data = result.ShouldBeOfType<Success<UserInfoResponse>>().Data.ShouldBeOfType<UserInfoResponse>();
        data.Email.ShouldBe(user.Email);
        data.Id.ShouldBe(user.Id);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_FailureInstance_ForNonExistingUser()
    {
        this.userContextMock.UserId.Returns(new AppUserId());

        var result = await this.sut.HandleAsync(new UserInfoCommand(), CancellationToken.None);

        result.ShouldBeOfType<Failure>();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_FailureInstance_WhenExceptionWasThrown()
    {
        this.TimeProviderMock.UtcNow.Throws(new InvalidOperationException());

        var command = new UserInfoCommand();
        var result = await this.sut.HandleAsync(command, CancellationToken.None);

        result.ShouldBeOfType<Failure>();
    }
}