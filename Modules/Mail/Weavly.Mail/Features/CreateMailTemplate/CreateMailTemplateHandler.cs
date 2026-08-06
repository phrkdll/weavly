using Weavly.Core.Shared.Contracts;
using Weavly.Mail.Shared.Features.CreateMailTemplate;

namespace Weavly.Mail.Features.CreateMailTemplate;

public sealed class CreateMailTemplateHandler : IWeavlyHandler<CreateMailTemplateCommand>
{
    public Task<Result> HandleAsync(CreateMailTemplateCommand command, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}