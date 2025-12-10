using Mapster;
using Microsoft.AspNetCore.Identity;
using Weavly.Auth.Enums;
using Weavly.Auth.Models;
using Weavly.Auth.Persistence;
using Weavly.Auth.Shared.Events;
using Weavly.Auth.Shared.Features.RegisterUser;
using Weavly.Core.Shared.Contracts;
using Weavly.Mail.Shared.Features.SendMail;
using Wolverine;

namespace Weavly.Auth.Features.RegisterUser;

public sealed class RegisterUserHandler(AuthDbContext dbContext, IPasswordHasher<AppUser> hasher, IMessageBus bus)
    : IWeavlyHandler<RegisterUserCommand, Result>
{
    public async Task<Result> HandleAsync(RegisterUserCommand command, CancellationToken ct = default)
    {
        try
        {
            if (dbContext.Users.Any(u => u.Email == command.Email))
            {
                return Failure.Create("Email is already in use.");
            }

            var user = AppUser.Create(command.Email, [AppUserToken.CreateEmailValidationToken()]);
            user.PasswordHash = hasher.HashPassword(user, command.Password);

            dbContext.Add(user);

            await dbContext.SaveChangesAsync(ct);

            await bus.PublishAsync(VerificationMail(user));
            await bus.PublishAsync(AppUserRegisteredEvent.Create(user.Id, user.Email));

            return Success.Create(user.Adapt<RegisterUserResponse>());
        }
        catch (Exception ex)
        {
            return Failure.Create(ex.Message);
        }
    }

    private static SendMailCommand VerificationMail(AppUser user)
    {
        var token = user.Tokens.Single(x => x.Purpose == AppUserTokenPurpose.EmailValidation).Value;

        const string subject = "Weavly verification mail";
        var body = $"""
            <p>Hi!</p>
            <p>Please verify your email address by clicking the link below:</p>
            <p><a href='http://localhost:5000/user/verify?token={token}'>Verify</a></p>
            """;

        return new SendMailCommand(user.Email, subject, body);
    }
}
