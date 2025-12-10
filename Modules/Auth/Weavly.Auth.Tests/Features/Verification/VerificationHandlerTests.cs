using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;
using Weavly.Auth.Features.Verification;
using Weavly.Auth.Models;
using Weavly.Auth.Shared.Features.Verification;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Implementation;
using Weavly.Core.Shared.Implementation;

namespace Weavly.Auth.Tests.Features.Verification;

public sealed class VerificationHandlerTests : AuthHandlerTests
{
    private readonly VerificationHandler sut;

    public VerificationHandlerTests()
    {
        sut = new VerificationHandler(dbContextMock, new DefaultTimeProvider(), messageBusMock);
    }

    [Fact]
    public async Task HandleAsync_ReturnsFailure_WhenUserNotFound()
    {
        userContextMock.UserId.Returns(new AppUserId());
        var result = await sut.HandleAsync(new VerificationCommand(Guid.NewGuid()));

        result.ShouldBeOfType<Failure>();
        result.Message.ShouldBe("Email address verification failed.");
    }

    [Fact]
    public async Task HandleAsync_ReturnsFailure_WhenTokenIsInvalid()
    {
        var user = AppUser.Create("admin@test.local", [AppUserToken.CreateEmailValidationToken()]);
        dbContextMock.Users.Add(user);
        await dbContextMock.SaveChangesAsync();

        userContextMock.UserId.Returns(user.Id);

        var result = await sut.HandleAsync(new VerificationCommand(Guid.NewGuid()));

        result.ShouldBeOfType<Failure>();
        result.Message.ShouldBe("Email address verification failed.");
    }

    [Fact]
    public async Task HandleAsync_ReturnsSuccess_WhenTokenIsValid()
    {
        var token = AppUserToken.CreateEmailValidationToken();
        dbContextMock.Users.Add(AppUser.Create("admin@test.local", [token]));
        await dbContextMock.SaveChangesAsync();

        var result = await sut.HandleAsync(new VerificationCommand(token.Value));

        result.ShouldBeOfType<Success<VerificationResponse>>();

        var user = await dbContextMock.Users.Include(x => x.Tokens).SingleAsync();
        user.Tokens.ShouldNotContain(token);
    }
}
