using NSubstitute;
using Shouldly;
using Weavly.Auth.Features.Verification;
using Weavly.Auth.Features.VerifyUser;
using Weavly.Auth.Models;
using Weavly.Auth.Shared.Features.VerifyUser;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Implementation;
using Weavly.Core.Shared.Contracts;
using Weavly.Core.Shared.Implementation;
using Weavly.Core.Shared.Models;

namespace Weavly.Auth.Tests.Features.Verification;

public sealed class VerifyUserHandlerTests : AuthHandlerTests
{
    private readonly VerifyUserHandler sut;
    private readonly IUserContext<AppUserId> userContextMock = Substitute.For<IUserContext<AppUserId>>();

    public VerifyUserHandlerTests()
    {
        sut = new VerifyUserHandler(Repository, new DefaultTimeProvider(), MessageBusMock);
    }

    [Fact]
    public async Task HandleAsync_ReturnsFailure_WhenUserNotFound()
    {
        userContextMock.UserId.Returns(new AppUserId());
        var result = await sut.HandleAsync(new VerifyUserCommand(Guid.NewGuid()));

        result.ShouldBeOfType<Failure>();
        result.Message.ShouldBe("Email address verification failed.");
    }

    [Fact]
    public async Task HandleAsync_ReturnsFailure_WhenTokenIsInvalid()
    {
        var user = AppUser.Create("admin@test.local", [AppUserToken.CreateEmailValidationToken()]);
        await Repository.Users.InsertAsync(user);

        userContextMock.UserId.Returns(user.Id);

        var result = await sut.HandleAsync(new VerifyUserCommand(Guid.NewGuid()));

        result.ShouldBeOfType<Failure>();
        result.Message.ShouldBe("Email address verification failed.");
    }

    [Fact]
    public async Task HandleAsync_ReturnsSuccess_WhenTokenIsValid()
    {
        var token = AppUserToken.CreateEmailValidationToken();
        await Repository.Users.InsertAsync(AppUser.Create("admin@test.local", [token]));

        var result = await sut.HandleAsync(new VerifyUserCommand(token.Value));

        result.ShouldBeOfType<Success<EmptyResponse>>();

        var user = await Repository.Users.FindAsync(x => x.Email == "admin@test.local");

        user.ShouldNotBeNull();
        user.Tokens.ShouldNotContain(token);
    }
}
