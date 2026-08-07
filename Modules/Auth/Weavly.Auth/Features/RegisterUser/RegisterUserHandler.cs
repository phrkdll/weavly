using Mapster;
using Microsoft.AspNetCore.Identity;
using Weavly.Auth.Enums;
using Weavly.Auth.Implementation;
using Weavly.Auth.Models;
using Weavly.Auth.Persistence;
using Weavly.Auth.Shared.Features.RegisterUser;
using Weavly.Configuration.Shared.Extensions;
using Weavly.Core.Shared.Contracts;
using Weavly.Mail.Shared.Features.SendMail;
using Wolverine;

namespace Weavly.Auth.Features.RegisterUser;

public sealed class RegisterUserHandler(
    AuthRepository repo,
    IPasswordHasher<AppUser> hasher,
    IValidator<RegisterUserCommand> validator,
    IMessageBus bus
) : IWeavlyHandler<RegisterUserCommand>
{
    public async Task<Result> HandleAsync(RegisterUserCommand command, CancellationToken ct = default)
    {
        try
        {
            using var session = await repo.StartTransactionAsync(ct);
            var config = await bus.LoadConfigurationAsync<AuthModule>(ct);
            var settings = GeneralSettings.FromConfigurationResponse(config);

            if (settings.DisableUserRegistration)
            {
                return Result.Failure("User registration is disabled.");
            }

            if (await validator.ValidateAsync(command, ct) is ValidationFailure validationFailure)
            {
                return validationFailure;
            }

            if (await IsEmailAddressAvailable(command) is Failure emailNotAvailable)
            {
                return emailNotAvailable;
            }

            var user = AppUser.Create(
                command.Email,
                settings.DisableEmailVerification ? [] : [AppUserToken.CreateEmailValidationToken()]
            );
            user.PasswordHash = hasher.HashPassword(user, command.Password);

            if (await repo.Roles.FindAsync(x => x.Name == "User", ct) is { } role)
            {
                user.Roles.Add(role.Id);
            }

            await repo.Users.InsertAsync(user, ct);

            await session.CommitTransactionAsync(ct);

            if (!settings.DisableEmailVerification)
            {
                var model = new
                {
                    BaseUrl = "http://localhost:5119",
                    Token = user.Tokens.Single(x => x.Purpose == AppUserTokenPurpose.EmailValidation).Value,
                };

                await bus.PublishAsync(SendMailCommand.Create<AuthModule>("RegisterUser", model, user.Email));
            }

            await bus.PublishAsync(AppUserRegisteredEvent.Create(user.Id, user.Email));

            return Result.Success(user.Adapt<RegisterUserResponse>());
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    private async Task<Result> IsEmailAddressAvailable(RegisterUserCommand request)
    {
        var users = await repo.Users.FilterAsync(x => x.Email == request.Email);

        return users.Any() ? Result.Failure("Email is already in use.") : Result.Success();
    }
}
