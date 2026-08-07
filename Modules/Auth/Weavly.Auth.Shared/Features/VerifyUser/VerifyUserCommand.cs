using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Shared.Features.VerifyUser;

public sealed record VerifyUserCommand(Guid Token) : IWeavlyCommand;
