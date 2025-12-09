using EntityFrameworkCore.Testing.NSubstitute;
using Microsoft.EntityFrameworkCore;
using Weavly.Core.Persistence;

namespace Weavly.Core.Tests;

public abstract class WeavlyHandlerTests<TDbContext> : WeavlyEndpointTests
    where TDbContext : CoreDbContext
{
    private readonly DbContextOptionsBuilder dbContextOptions = new DbContextOptionsBuilder()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .UseStronglyTypeConverters();

    protected readonly TDbContext dbContextMock;

    public WeavlyHandlerTests()
    {
        dbContextMock = Create.MockedDbContextFor<TDbContext>(dbContextOptions.Options);
    }
}
