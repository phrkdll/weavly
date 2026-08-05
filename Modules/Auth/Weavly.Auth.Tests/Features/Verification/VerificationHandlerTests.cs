using NSubstitute;
using Shouldly;
using Weavly.Auth.Features.Verification;
using Weavly.Auth.Models;
using Weavly.Auth.Shared.Features.Verification;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Implementation;
using Weavly.Core.Shared.Contracts;
using Weavly.Core.Shared.Implementation;

namespace Weavly.Auth.Tests.Features.Verification;

public sealed class VerificationHandlerTests : AuthHandlerTests
{
    private readonly VerificationHandler sut;
    private readonly IUserContext<AppUserId> userContextMock = Substitute.For<IUserContext<AppUserId>>();

    public VerificationHandlerTests()
    {
        this.sut = new VerificationHandler(this.Repository, new DefaultTimeProvider(), this.MessageBusMock);
    }

    [Fact]
    public async Task HandleAsync_ReturnsFailure_WhenUserNotFound()
    {
        this.userContextMock.UserId.Returns(new AppUserId());
        var result = await this.sut.HandleAsync(new VerificationCommand(Guid.NewGuid()));

        result.ShouldBeOfType<Failure>();
        result.Message.ShouldBe("Email address verification failed.");
    }

    [Fact]
    public async Task HandleAsync_ReturnsFailure_WhenTokenIsInvalid()
    {
        var user = AppUser.Create("admin@test.local", [AppUserToken.CreateEmailValidationToken()]);
        await this.Repository.Users.InsertAsync(user);

        this.userContextMock.UserId.Returns(user.Id);

        var result = await this.sut.HandleAsync(new VerificationCommand(Guid.NewGuid()));

        result.ShouldBeOfType<Failure>();
        result.Message.ShouldBe("Email address verification failed.");
    }

    [Fact]
    public async Task HandleAsync_ReturnsSuccess_WhenTokenIsValid()
    {
        var token = AppUserToken.CreateEmailValidationToken();
        await this.Repository.Users.InsertAsync(AppUser.Create("admin@test.local", [token]));

        var result = await this.sut.HandleAsync(new VerificationCommand(token.Value));

        result.ShouldBeOfType<Success<VerificationResponse>>();

        var user = await this.Repository.Users.FindAsync(x => x.Email == "admin@test.local");

        user.ShouldNotBeNull();
        user.Tokens.ShouldNotContain(token);
    }
}
