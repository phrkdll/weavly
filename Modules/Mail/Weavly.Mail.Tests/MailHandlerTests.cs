using Weavly.Mail.Persistence;
using Weavly.Core.Tests.Shared;

namespace Weavly.Mail.Tests;

public abstract class MailHandlerTests : WeavlyHandlerTests
{
    protected readonly MailRepository Repository;

    protected MailHandlerTests()
    {
        this.Repository = new MailRepository(
            new WeavlyRepositoryMock<MailModule>(this.TimeProviderMock)
        );
    }
}
