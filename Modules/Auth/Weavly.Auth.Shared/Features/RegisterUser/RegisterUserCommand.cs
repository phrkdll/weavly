using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Shared.Features.RegisterUser;

public sealed record RegisterUserCommand(string Email, string Password) : IWeavlyCommand;
