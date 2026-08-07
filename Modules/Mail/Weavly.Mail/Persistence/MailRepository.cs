using Weavly.Core.Shared.Persistence;
using Weavly.Mail.Models;
using Weavly.Mail.Shared.Identifiers;

namespace Weavly.Mail.Persistence;

public class MailRepository(IWeavlyRepository<MailModule> repo) : ModuleRepository<MailModule>(repo)
{
    public IWeavlyCollection<MailTemplate, MailTemplateId> MailTemplates =>
        this.Repository.GetCollectionFor<MailTemplate, MailTemplateId>();
}
