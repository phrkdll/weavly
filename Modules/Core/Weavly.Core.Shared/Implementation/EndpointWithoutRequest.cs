using System.Net;
using FastEndpoints;

namespace Weavly.Core.Shared.Implementation;

public abstract class EndpointWithoutRequest : EndpointWithoutRequest<Result>
{
    protected Task SendConflictAsync(Result response, CancellationToken ct)
    {
        return SendAsync(response, (int)HttpStatusCode.Conflict, ct);
    }

    protected Task SendBadRequestAsync(Result response, CancellationToken ct)
    {
        return SendAsync(response, (int)HttpStatusCode.BadRequest, ct);
    }

    protected Task SendUnauthorizedAsync(Result response, CancellationToken ct)
    {
        return SendAsync(response, (int)HttpStatusCode.Unauthorized, ct);
    }

    public abstract override void Configure();

    public abstract override Task HandleAsync(CancellationToken ct);

    protected async Task HandleDefaultAsync<T>(T request, CancellationToken ct)
        where T : notnull, ICommand<Result>
    {
        var response = await request.ExecuteAsync(ct);

        if (response.Success)
        {
            await SendOkAsync(response, ct);
        }
        else
        {
            await SendBadRequestAsync(response, ct);
        }
    }
}
