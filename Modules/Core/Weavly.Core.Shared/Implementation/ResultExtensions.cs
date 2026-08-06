using System.ComponentModel.DataAnnotations;
using Weavly.Core.Shared.Models;

namespace Weavly.Core.Shared.Implementation;

public static class ResultExtensions
{
    extension(Result)
    {
        public static Success<T> Success<T>(T data, string? message = null)
        {
            return new Success<T>(data, message);
        }

        public static Success<EmptyResponse> Success(string? message = null)
        {
            return new Success<EmptyResponse>(null!, message);
        }

        public static Failure Failure(string message)
        {
            return new Failure(message);
        }

        public static Failure Failure(Exception exception, string? message = null)
        {
            return new Failure(message ?? exception.Message, exception);
        }

        public static Failure ValidationFailure(IEnumerable<ValidationResult?> validationResults)
        {
            return new ValidationFailure(validationResults);
        }
    }
}
