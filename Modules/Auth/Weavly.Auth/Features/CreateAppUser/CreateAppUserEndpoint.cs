using Weavly.Auth.Shared.Features.CreateAppUser;
using Weavly.Core.Shared.Implementation.Endpoints;
using Wolverine;

namespace Weavly.Auth.Features.CreateAppUser;

public sealed class CreateAppUserEndpoint(IMessageBus bus)
    : PostEndpoint<CreateAppUserCommand, AuthModule>("user", bus);
