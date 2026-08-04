using System.ComponentModel.DataAnnotations;

namespace Weavly.Core.Shared.Implementation;

public record ValidationFailure(IEnumerable<ValidationResult?> Results) : Failure("Validation failed");