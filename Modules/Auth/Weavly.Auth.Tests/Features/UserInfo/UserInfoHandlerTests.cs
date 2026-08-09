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
        sut = new UserInfoHandler(Repository, userContextMock);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_SuccessInstance_ForExistingUser()
    {
        var user = AppUser.Create("admin@test.local", []);
        await Repository.Users.InsertAsync(user);

        userContextMock.UserId.Returns(user.Id);

        var result = await sut.HandleAsync(new UserInfoCommand(), CancellationToken.None);

        var data = result.ShouldBeOfType<Success<UserInfoResponse>>().Data.ShouldBeOfType<UserInfoResponse>();
        data.Email.ShouldBe(user.Email);
        data.Id.ShouldBe(user.Id);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_FailureInstance_ForNonExistingUser()
    {
        userContextMock.UserId.Returns(new AppUserId());

        var result = await sut.HandleAsync(new UserInfoCommand(), CancellationToken.None);

        result.ShouldBeOfType<Failure>();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_FailureInstance_WhenExceptionWasThrown()
    {
        TimeProviderMock.UtcNow.Throws(new InvalidOperationException());

        var command = new UserInfoCommand();
        var result = await sut.HandleAsync(command, CancellationToken.None);

        result.ShouldBeOfType<Failure>();
    }
}
