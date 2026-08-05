using Weavly.Core.Shared.Contracts;

namespace Weavly.Core.Tests;

public record struct TestId(string Value) : IWeavlyId;
