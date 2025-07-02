using FastEndpoints;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Weavly.Auth.Enums;
using Weavly.Auth.Models;
using Weavly.Auth.Persistence;
using Weavly.Auth.Shared.Events;
using Weavly.Auth.Shared.Features.Register;
using Weavly.Mail.Shared.Triggers;

namespace Weavly.Auth.Features.Register;

public sealed class RegisterUserCommandHandler(AuthDbContext dbContext, PasswordHasher<AppUser> hasher)
    : ICommandHandler<RegisterUserCommand, Result>
{
    public async Task<Result> ExecuteAsync(RegisterUserCommand command, CancellationToken ct)
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

            await VerificationMail(user).ExecuteAsync(ct);
            await AppUserRegisteredEvent.Create(user.Id, user.Email).PublishAsync(Mode.WaitForNone, ct);

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
