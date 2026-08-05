using Weavly.Configuration.Shared.Identifiers;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Configuration.Shared.Features.UpdateConfiguration;

public sealed record UpdateConfigurationCommand(
    ConfigurationId Id,
    bool? BoolValue,
    int? IntValue,
    string? StringValue,
    double? DoubleValue
) : IWeavlyCommand;
