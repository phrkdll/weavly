namespace Weavly.Core.Shared.Contracts;

public interface IValidator<in T>
{
    Task<Result> ValidateAsync(T obj, CancellationToken ct = default);
}
