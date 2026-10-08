using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Shared.Features.SeedAppUser;

public sealed record SeedAppUserCommand(string Email, string UserName, string InitialRole) : IWeavlyCommand;
