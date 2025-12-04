using System.Net;

namespace Weavly.Core.Shared.Implementation;

public abstract class Endpoint<TRequest> : Endpoint<TRequest, Result>
    where TRequest : notnull
{
    protected Task SendConflictAsync(Result response, CancellationToken ct)
    {
        return Send.ResponseAsync(response, (int)HttpStatusCode.Conflict, ct);
    }

    protected Task SendBadRequestAsync(Result response, CancellationToken ct)
    {
        return Send.ResponseAsync(response, (int)HttpStatusCode.BadRequest, ct);
    }

    protected Task SendUnauthorizedAsync(Result response, CancellationToken ct)
    {
        return Send.ResponseAsync(response, (int)HttpStatusCode.Unauthorized, ct);
    }

    public abstract override void Configure();

    public abstract override Task HandleAsync(TRequest req, CancellationToken ct);

    protected async Task HandleDefaultAsync<T>(T request, CancellationToken ct)
        where T : notnull, TRequest
    {
        if (request is not ICommand<Result> command)
        {
            await SendBadRequestAsync(Failure.Create("Request is not a command"), ct);

            return;
        }

        var response = await command.ExecuteAsync(ct);

        if (response.Success)
        {
            await Send.OkAsync(response, ct);
        }
        else
        {
            await SendBadRequestAsync(response, ct);
        }
    }
}
