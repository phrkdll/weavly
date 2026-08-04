using Weavly.Auth.Shared.Features.RegisterUser;
using Weavly.Core.Shared.Implementation.Endpoints;
using Wolverine;

namespace Weavly.Auth.Features.RegisterUser;

public sealed class RegisterUserEndpoint(IMessageBus bus)
    : PostEndpoint<RegisterUserCommand, RegisterUserResponse, AuthModule>("user/register", bus);