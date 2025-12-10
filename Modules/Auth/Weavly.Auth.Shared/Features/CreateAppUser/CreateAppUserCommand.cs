using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Shared.Features.CreateAppUser;

public sealed record CreateAppUserCommand(string Email, string UserName, string InitialRole, string Password = "")
    : IWeavlyCommand;
