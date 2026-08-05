using Weavly.Auth.Shared.Features.CreateAppUser;
using Weavly.Core.Shared.Implementation.Endpoints;
using Weavly.Core.Shared.Models;
using Wolverine;

namespace Weavly.Auth.Features.CreateAppUser;

public sealed class CreateAppUserEndpoint(IMessageBus bus)
    : PostEndpoint<CreateAppUserCommand, EmptyResponse, AuthModule>("user", bus);
