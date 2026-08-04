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

    private readonly IMongoCollection<TestDocument> mongoCollectionMock =
        Substitute.For<IMongoCollection<TestDocument>>();

    private readonly WeavlyCollection<TestDocument, TestId> sut;
    private readonly TestId testId = new(ObjectId.GenerateNewId().ToString());

    private readonly ITimeProvider timeProviderMock = Substitute.For<ITimeProvider>();

    public WeavlyCollectionTests()
    {
        this.mongoCollectionMock
            .FindAsync(Arg.Any<FilterDefinition<TestDocument>>(),
                Arg.Any<FindOptions<TestDocument, TestDocument>>(), Arg.Any<CancellationToken>())
            .Returns(this.asyncCursorMock);

        this.sut = new WeavlyCollection<TestDocument, TestId>(this.mongoCollectionMock, this.timeProviderMock);
    }

    [Fact]
    public async Task InsertAsync_ShouldSetMetadata_AndCallInsertOneAsync()
    {
        var testTime = DateTime.UtcNow;
        this.timeProviderMock.UtcNow.Returns(testTime);

        var document = new TestDocument("Test");
        await this.sut.InsertAsync(document, this.cancellationToken);

        document.CreatedAt.ShouldBe(testTime);
        document.TouchedAt.ShouldBe(testTime);
        document.Name.ShouldBe("Test");

        await this.mongoCollectionMock.Received(1)
            .InsertOneAsync(document, Arg.Any<InsertOneOptions>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_ShouldSetMetadata_AndCallReplaceOneAsync()
    {
        var testTime = DateTime.UtcNow;
        this.timeProviderMock.UtcNow.Returns(testTime);

        var document = new TestDocument("Test")
            { Id = this.testId, CreatedAt = this.createTime, TouchedAt = this.createTime };
        await this.sut.UpdateAsync(document, this.cancellationToken);

        document.CreatedAt.ShouldBe(this.createTime);
        document.TouchedAt.ShouldBe(testTime);
        document.Name.ShouldBe("Test");

        await this.mongoCollectionMock.Received(1)
            .ReplaceOneAsync(Arg.Any<FilterDefinition<TestDocument>>(), document,
                null as ReplaceOptions, this.cancellationToken);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSetDeletedAt_AndCallReplaceOneAsync()
    {
        var testTime = DateTime.UtcNow;
        this.timeProviderMock.UtcNow.Returns(testTime);

        var document = new TestDocument("Test")
            { Id = this.testId, CreatedAt = this.createTime, TouchedAt = this.createTime };

        this.asyncCursorMock.MoveNextAsync(Arg.Any<CancellationToken>()).Returns(true);
        this.asyncCursorMock.Current.Returns([document]);

        await this.sut.DeleteAsync(this.testId, this.cancellationToken);

        document.CreatedAt.ShouldBe(this.createTime);
        document.TouchedAt.ShouldBe(this.createTime);
        document.DeletedAt.ShouldBe(testTime);
        document.Name.ShouldBe("Test");

        await this.mongoCollectionMock.Received(1)
            .ReplaceOneAsync(Arg.Any<FilterDefinition<TestDocument>>(), document,
                null as ReplaceOptions, this.cancellationToken);
    }
}