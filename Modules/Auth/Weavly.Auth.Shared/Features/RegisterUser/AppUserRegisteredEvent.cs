using System.Text.Json.Serialization;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Shared.Features.RegisterUser;

public sealed class AppUserRegisteredEvent : IWeavlyEvent
{
    [JsonConstructor]
    private AppUserRegisteredEvent(AppUserId? id, string email)
    {
        Id = id;
        Email = email;
    }

    public AppUserId? Id { get; }
    public string Email { get; }

    public static AppUserRegisteredEvent Create(AppUserId? id, string email)
    {
        return new AppUserRegisteredEvent(id, email);
    }
}
