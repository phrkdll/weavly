using System.Text.Json.Serialization;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Shared.Events;

public sealed class AppUserRegisteredEvent : IWeavlyEvent
{
    public AppUserId? Id { get; }
    public string Email { get; }

    [JsonConstructor]
    private AppUserRegisteredEvent(AppUserId? id, string email)
    {
        Id = id;
        Email = email;
    }

    public static AppUserRegisteredEvent Create(AppUserId? id, string email) => new(id, email);
}
