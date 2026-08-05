using Weavly.Core.Shared.Contracts;

namespace Weavly.Messages.Shared.Identifiers;

public record struct MessageId(string Value) : IWeavlyId;
