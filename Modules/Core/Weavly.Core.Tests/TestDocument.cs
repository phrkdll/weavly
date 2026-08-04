using Weavly.Core.Shared.Models;

namespace Weavly.Core.Tests;

public sealed record TestDocument(string Name) : Document<TestId>;