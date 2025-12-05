using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Shared.Features.LoginUser;

public sealed record LoginUserCommand(string Email, string Password) : IWeavlyCommand;
