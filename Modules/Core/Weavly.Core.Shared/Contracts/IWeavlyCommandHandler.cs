namespace Weavly.Core.Shared.Contracts;

public interface IWeavlyHandler<TCommand, TResponse> : IWeavlyHandler
    where TCommand : IWeavlyCommand
{
    Task<TResponse> HandleAsync(TCommand command, CancellationToken ct);
}

public interface IWeavlyHandler;
