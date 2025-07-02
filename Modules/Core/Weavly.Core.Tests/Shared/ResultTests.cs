using Weavly.Core.Shared.Implementation.Results;
using Shouldly;

namespace Weavly.Core.Tests.Shared;

public sealed class ResultTests
{
    [Theory]
    [ClassData(typeof(SuccessFactoryTestData))]
    public void SuccessFactory_Should_Return_Success_Instance(object data, string? message)
    {
        var result = Success.Create(data, message);

        result.ShouldNotBeNull();

        result.Success.ShouldBeTrue();

        result.Data.ShouldNotBeNull();
        result.Data.ShouldBe(data);

        result.Message.ShouldBe(message);
    }

    [Theory]
    [ClassData(typeof(FailureFactoryTestData))]
    public void FailureFactory_Should_Return_Failure_Instance(string message)
    {
        var result = Failure.Create(message);

        result.ShouldNotBeNull();
        result.ShouldBeOfType<Failure>();

        result.Success.ShouldBeFalse();

        result.Message.ShouldBe(message);
    }

    internal class SuccessFactoryTestData : TheoryData<object, string>
    {
        public SuccessFactoryTestData()
        {
            Add("Data", "Message");
            Add(new FileInfo("test.txt"), "File found");
        }
    }

    internal class FailureFactoryTestData : TheoryData<string>
    {
        public FailureFactoryTestData()
        {
            Add("Message");
            Add("File not found");
        }
    }
}
