using Strongly;

namespace Weavly.Configuration.Shared.Identifiers;

[Strongly(StronglyType.String, StronglyConverter.EfValueConverter)]
public readonly partial record struct ConfigurationId;
