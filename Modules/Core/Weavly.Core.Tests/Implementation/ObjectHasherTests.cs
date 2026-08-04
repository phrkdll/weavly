using Shouldly;
using Weavly.Core.Implementation;

namespace Weavly.Core.Tests.Implementation;

public class ObjectHasherTests
{
    private readonly ObjectHasher sut = new();

    [Theory]
    [MemberData(nameof(TestData))]
    public void ComputeHash_ShouldHash_AnyObject(object obj)
    {
        var hash = this.sut.ComputeHash(obj);

        hash.ShouldNotBeNullOrWhiteSpace();
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void ComputeHash_ShouldCreate_ConsistentHashes(object obj)
    {
        var a = this.sut.ComputeHash(obj);
        var b = this.sut.ComputeHash(obj);

        a.ShouldBe(b);
    }

    [Theory]
    [MemberData(nameof(DifferenceTestData))]
    public void ComputeHash_ShouldCreate_DifferentHashes_EvenWithMinimalObjectDifferences(object a, object b)
    {
        var hashA = this.sut.ComputeHash(a);
        var hashB = this.sut.ComputeHash(b);

        hashA.ShouldNotBe(hashB);
    }

    public static TheoryData<object> TestData()
    {
        return [new MongoDbOptions(), "Fresh", new TestDocument("Fresh")];
    }

    public static TheoryData<object, object> DifferenceTestData()
    {
        return new TheoryData<object, object>
        {
            { "Fresh", "fresh" },
            { new TestDocument("Fresh"), new TestDocument("fresh") },
            {
                new MongoDbOptions { ConnectionString = "mongodb://localhost:27017" },
                new TestDocument("mongodb://localhost:27018")
            }
        };
    }
}