using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Weavly.Auth.Implementation;
using Weavly.Auth.Shared.Features.RegisterUser;
using Weavly.Configuration.Shared.Extensions;
using Weavly.Core.Shared.Contracts;
using Wolverine;

namespace Weavly.Auth.Features.RegisterUser;

public partial class RegisterUserCommandValidator(IMessageBus bus) : IValidator<RegisterUserCommand>
{
    public async Task<Result> ValidateAsync(RegisterUserCommand command, CancellationToken ct = default)
    {
        var options = PasswordRules.FromConfigurationResponse(await bus.LoadConfigurationAsync<AuthModule>(ct));

        IList<ValidationResult?> results =
        [
            ValidateEmail(command),
            .. ValidateLength(command, options),
            .. ValidateContains(command, options)
        ];

        return results.Any(x => x != null)
            ? Result.ValidationFailure(results.Where(x => x != null))
            : Result.Success();
    }

    [GeneratedRegex("(?=.*[A-Z])")]
    private static partial Regex ContainsUppercase();

    [GeneratedRegex("(?=.*[a-z])")]
    private static partial Regex ContainsLowercase();

    [GeneratedRegex("(?=.*[0-9])")]
    private static partial Regex ContainsDigit();

    [GeneratedRegex(@"(?=.*[!@#$%^&*_\-\.])")]
    private static partial Regex ContainsNonAlphanumeric();

    private static ValidationResult? ValidateEmail(RegisterUserCommand command)
    {
        try
        {
            return new MailAddress(command.Email).Address == command.Email
                ? ValidationResult.Success
                : new ValidationResult("Invalid email address.");
        }
        catch (FormatException)
        {
            return new ValidationResult("Invalid email format.");
        }
    }

    private static IEnumerable<ValidationResult?> ValidateContains(RegisterUserCommand command, PasswordRules rules)
    {
        yield return rules.RequireUppercase && !ContainsUppercase().IsMatch(command.Password)
            ? new ValidationResult("Password must contain at least one uppercase character.")
            : ValidationResult.Success;

        yield return rules.RequireLowercase && !ContainsLowercase().IsMatch(command.Password)
            ? new ValidationResult("Password must contain at least one lowercase character.")
            : ValidationResult.Success;

        yield return rules.RequireDigit && !ContainsDigit().IsMatch(command.Password)
            ? new ValidationResult("Password must contain at least one digit.")
            : ValidationResult.Success;

        yield return rules.RequireNonAlphanumeric && !ContainsNonAlphanumeric().IsMatch(command.Password)
            ? new ValidationResult("Password must contain at least one non alphanumeric character.")
            : ValidationResult.Success;
    }

    private static IEnumerable<ValidationResult?> ValidateLength(RegisterUserCommand command,
        PasswordRules rules)
    {
        yield return command.Password.Length < rules.MinimumLength
            ? new ValidationResult($"Password length must not be less than {rules.MinimumLength} characters.")
            : ValidationResult.Success;

        yield return command.Password.Length > rules.MaximumLength
            ? new ValidationResult($"Password length must not exceed {rules.MaximumLength} characters.")
            : ValidationResult.Success;
    }
}