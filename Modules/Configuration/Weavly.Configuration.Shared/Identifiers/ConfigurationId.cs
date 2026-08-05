using Weavly.Core.Shared.Contracts;

namespace Weavly.Configuration.Shared.Identifiers;

public record struct ConfigurationId(string Value) : IWeavlyId;
