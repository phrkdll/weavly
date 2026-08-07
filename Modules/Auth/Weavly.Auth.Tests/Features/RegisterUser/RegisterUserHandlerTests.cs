using Microsoft.AspNetCore.Identity;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using Weavly.Auth.Enums;
using Weavly.Auth.Features.RegisterUser;
using Weavly.Auth.Models;
using Weavly.Auth.Shared.Features.RegisterUser;
using Weavly.Core.Shared.Contracts;
using Weavly.Core.Shared.Implementation;
using Weavly.Mail.Shared.Features.SendMail;

namespace Weavly.Auth.Tests.Features.RegisterUser;

public sealed class RegisterUserHandlerTests : AuthHandlerTests
{
    private readonly RegisterUserHandler sut;
    private readonly IValidator<RegisterUserCommand> validator = Substitute.For<IValidator<RegisterUserCommand>>();

    public RegisterUserHandlerTests()
    {
        this.TimeProviderMock.UtcNow.Returns(DateTime.UtcNow);

        this.sut = new RegisterUserHandler(
            this.Repository,
            new PasswordHasher<AppUser>(),
            this.validator,
            this.MessageBusMock
        );
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_SuccessInstance_WhenNewUserWasCreated()
    {
        this.validator.ValidateAsync(Arg.Any<RegisterUserCommand>()).Returns(Result.Success());

        var command = new RegisterUserCommand("admin@test.local", "P@ssw0rd!");
        var result = await this.sut.HandleAsync(command, CancellationToken.None);
        var data = result.ShouldBeOfType<Success<RegisterUserResponse>>().Data;

        data.ShouldNotBeNull();

        var user = await this.Repository.Users.FindAsync(x => true, CancellationToken.None);
        user.ShouldNotBeNull();
        user.Tokens.ShouldContain(t => t.Purpose == AppUserTokenPurpose.EmailValidation);
        user.PasswordHash.ShouldNotBeNullOrEmpty();
        user.PasswordHash.ShouldNotBe(command.Password);

        await this.MessageBusMock.Received().PublishAsync(Arg.Any<SendMailCommand>());
        await this.MessageBusMock.Received().PublishAsync(Arg.Any<AppUserRegisteredEvent>());
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_ValidationFailure_WhenValidationFailed()
    {
        this.validator.ValidateAsync(Arg.Any<RegisterUserCommand>()).Returns(Result.ValidationFailure([]));

        var command = new RegisterUserCommand("admin@test.local", "P@ssw0rd!");
        var result = await this.sut.HandleAsync(command, CancellationToken.None);
        var (message, _) = result.ShouldBeOfType<ValidationFailure>();

        message.ShouldBe("Validation failed");

        await this.MessageBusMock.DidNotReceive().PublishAsync(Arg.Any<SendMailCommand>());
        await this.MessageBusMock.DidNotReceive().PublishAsync(Arg.Any<AppUserRegisteredEvent>());
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_FailureInstance_WhenUserAlreadyExists()
    {
        var user = AppUser.Create("admin@test.local", [AppUserToken.CreateEmailValidationToken()]);
        await this.Repository.Users.InsertAsync(user, CancellationToken.None);

        var command = new RegisterUserCommand("admin@test.local", "P@ssw0rd!");
        var result = await this.sut.HandleAsync(command, CancellationToken.None);

        result.ShouldBeOfType<Failure>();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_FailureInstance_WhenExceptionWasThrown()
    {
        this.TimeProviderMock.UtcNow.Throws(new Exception("Could not retrieve UTC time"));

        var command = new RegisterUserCommand("admin@test.local", "P@ssw0rd!");
        var result = await this.sut.HandleAsync(command, CancellationToken.None);

        result.ShouldBeOfType<Failure>();
    }
}
