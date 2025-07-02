using System.Text.Json.Serialization;
using FastEndpoints;
using Weavly.Auth.Shared.Identifiers;

namespace Weavly.Auth.Shared.Events;

public sealed class AppUserRegisteredEvent : IEvent
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
