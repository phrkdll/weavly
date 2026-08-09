using MongoDB.Bson;
using MongoDB.Driver;
using NSubstitute;
using Shouldly;
using Weavly.Core.Shared.Contracts;
using Weavly.Core.Shared.Persistence;

namespace Weavly.Core.Tests.Shared.Persistence;

public sealed class WeavlyCollectionTests
{
    private readonly IAsyncCursor<TestDocument> asyncCursorMock = Substitute.For<IAsyncCursor<TestDocument>>();
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private readonly DateTime createTime = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private readonly IMongoCollection<TestDocument> mongoCollectionMock = Substitute.For<
        IMongoCollection<TestDocument>
    >();

    private readonly WeavlyCollection<TestDocument, TestId> sut;
    private readonly TestId testId = new(ObjectId.GenerateNewId().ToString());

    private readonly ITimeProvider timeProviderMock = Substitute.For<ITimeProvider>();

    public WeavlyCollectionTests()
    {
        mongoCollectionMock.FindAsync(
                Arg.Any<FilterDefinition<TestDocument>>(),
                Arg.Any<FindOptions<TestDocument, TestDocument>>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(asyncCursorMock);

        sut = new WeavlyCollection<TestDocument, TestId>(mongoCollectionMock, timeProviderMock);
    }

    [Fact]
    public async Task InsertAsync_ShouldSetMetadata_AndCallInsertOneAsync()
    {
        var testTime = DateTime.UtcNow;
        timeProviderMock.UtcNow.Returns(testTime);

        var document = new TestDocument("Test");
        await sut.InsertAsync(document, cancellationToken);

        document.CreatedAt.ShouldBe(testTime);
        document.TouchedAt.ShouldBe(testTime);
        document.Name.ShouldBe("Test");

        await mongoCollectionMock.Received(1)
            .InsertOneAsync(document, Arg.Any<InsertOneOptions>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_ShouldSetMetadata_AndCallReplaceOneAsync()
    {
        var testTime = DateTime.UtcNow;
        timeProviderMock.UtcNow.Returns(testTime);

        var document = new TestDocument("Test")
        {
            Id = testId,
            CreatedAt = createTime,
            TouchedAt = createTime,
        };
        await sut.UpdateAsync(document, cancellationToken);

        document.CreatedAt.ShouldBe(createTime);
        document.TouchedAt.ShouldBe(testTime);
        document.Name.ShouldBe("Test");

        await mongoCollectionMock.Received(1)
            .ReplaceOneAsync(
                Arg.Any<FilterDefinition<TestDocument>>(),
                document,
                null as ReplaceOptions,
                cancellationToken
            );
    }

    [Fact]
    public async Task DeleteAsync_ShouldSetDeletedAt_AndCallReplaceOneAsync()
    {
        var testTime = DateTime.UtcNow;
        timeProviderMock.UtcNow.Returns(testTime);

        var document = new TestDocument("Test")
        {
            Id = testId,
            CreatedAt = createTime,
            TouchedAt = createTime,
        };

        asyncCursorMock.MoveNextAsync(Arg.Any<CancellationToken>()).Returns(true);
        asyncCursorMock.Current.Returns([document]);

        await sut.DeleteAsync(testId, cancellationToken);

        document.CreatedAt.ShouldBe(createTime);
        document.TouchedAt.ShouldBe(createTime);
        document.DeletedAt.ShouldBe(testTime);
        document.Name.ShouldBe("Test");

        await mongoCollectionMock.Received(1)
            .ReplaceOneAsync(
                Arg.Any<FilterDefinition<TestDocument>>(),
                document,
                null as ReplaceOptions,
                cancellationToken
            );
    }
}
