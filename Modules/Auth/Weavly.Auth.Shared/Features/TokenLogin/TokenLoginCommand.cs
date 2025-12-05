using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Shared.Features.TokenLogin;

public sealed record TokenLoginCommand(Guid Token) : IWeavlyCommand;
