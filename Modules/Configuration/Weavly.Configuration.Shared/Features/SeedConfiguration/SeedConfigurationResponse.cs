using Weavly.Configuration.Shared.Identifiers;

namespace Weavly.Configuration.Shared.Features.SeedConfiguration;

public sealed record SeedConfigurationResponse(ConfigurationId Id, bool Created);
