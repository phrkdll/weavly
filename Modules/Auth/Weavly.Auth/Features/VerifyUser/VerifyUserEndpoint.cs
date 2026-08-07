using Weavly.Auth.Shared.Features.VerifyUser;
using Weavly.Core.Shared.Implementation.Endpoints;
using Weavly.Core.Shared.Models;
using Wolverine;

namespace Weavly.Auth.Features.Verification;

public sealed class VerifyUserEndpoint(IMessageBus bus)
    : GetEndpoint<VerifyUserCommand, EmptyResponse, AuthModule>("user/verify", bus);
