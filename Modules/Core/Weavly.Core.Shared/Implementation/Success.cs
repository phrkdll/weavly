using System.Diagnostics.CodeAnalysis;

namespace Weavly.Core.Shared.Implementation;

public record Success<T>(T Data) : Result(true, null);
