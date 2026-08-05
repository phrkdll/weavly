using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Shared.Features.Verification;

public sealed record VerificationCommand(Guid Token) : IWeavlyCommand;
