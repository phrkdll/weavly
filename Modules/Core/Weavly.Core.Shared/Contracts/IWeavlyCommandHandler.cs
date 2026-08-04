namespace Weavly.Core.Shared.Contracts;

public interface IWeavlyHandler<in TCommand, TResponse> : IWeavlyHandler
    where TCommand : IWeavlyCommand
{
    Task<TResponse> HandleAsync(TCommand command, CancellationToken ct = default);
}

public interface IWeavlyHandler;