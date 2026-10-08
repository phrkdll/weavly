using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Weavly.Core.Shared.Contracts;
using Weavly.Core.Shared.Implementation;
using Weavly.Core.Tests.Shared;
using Weavly.Mail.Features.SeedMailTemplate;
using Weavly.Mail.Persistence;
using Weavly.Mail.Shared.Features.SeedMailTemplate;

namespace Weavly.Mail.Tests;

public sealed class SeedMailTemplateHandlerTests
{
    private readonly ITimeProvider timeProvider = Substitute.For<ITimeProvider>();
    private readonly MailRepository repository;
    private readonly SeedMailTemplateHandler sut;

    public SeedMailTemplateHandlerTests()
    {
        timeProvider.UtcNow.Returns(DateTime.UtcNow);
        repository = new MailRepository(new WeavlyRepositoryMock<MailModule>(timeProvider));
        sut = new SeedMailTemplateHandler(repository, Substitute.For<ILogger<SeedMailTemplateHandler>>());
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateMissingTemplate_AndPreserveItOnRepeatedSeed()
    {
        var command = SeedMailTemplateCommand.Create<SeedMailTemplateHandlerTests>("Name", "original", "original text");
        var created = (await sut.HandleAsync(command)).ShouldBeOfType<Success<SeedMailTemplateResponse>>().Data;
        var repeated = (
            await sut.HandleAsync(command with { Subject = "replacement", Text = "replacement text" })
        ).ShouldBeOfType<Success<SeedMailTemplateResponse>>().Data;
        var persisted = await repository.MailTemplates.FindAsync(x =>
            x.Module == nameof(SeedMailTemplateHandlerTests) && x.Name == "Name"
        );

        created.Created.ShouldBeTrue();
        repeated.Created.ShouldBeFalse();
        repeated.MailTemplateId.ShouldBe(created.MailTemplateId);
        persisted.ShouldNotBeNull();
        persisted.Subject.ShouldBe("original");
        persisted.Text.ShouldBe("original text");
    }
}
