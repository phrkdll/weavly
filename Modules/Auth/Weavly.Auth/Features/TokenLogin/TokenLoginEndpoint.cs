using Weavly.Auth.Shared.Features.TokenLogin;
using Weavly.Core.Shared.Implementation.Endpoints;
using Wolverine;

namespace Weavly.Auth.Features.TokenLogin;

internal sealed class TokenLoginEndpoint(IMessageBus bus)
    : GetEndpoint<TokenLoginCommand, AuthModule>("user/login", bus);
