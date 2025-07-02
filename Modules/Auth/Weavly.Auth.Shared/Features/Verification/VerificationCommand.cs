using FastEndpoints;

namespace Weavly.Auth.Shared.Features.Verification;

public sealed record VerificationCommand(Guid Token) : ICommand<Result>;
