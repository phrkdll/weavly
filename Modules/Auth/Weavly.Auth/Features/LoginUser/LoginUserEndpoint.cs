using Weavly.Auth.Shared.Features.LoginUser;
using Weavly.Core.Shared.Implementation.Endpoints;
using Wolverine;

namespace Weavly.Auth.Features.LoginUser;

public sealed class LoginUserEndpoint(IMessageBus bus) : PostEndpoint<LoginUserCommand, AuthModule>("user/login", bus);
