using Shouldly;
using Weavly.Core.Shared.Implementation.Results;

namespace Weavly.Core.Tests.Shared;

public sealed class ResultTests
{
    [Theory]
    [ClassData(typeof(SuccessFactoryTestData))]
    public void SuccessFactory_ShouldReturn_SuccessInstance(string data, string? message)
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
    public void FailureFactory_ShouldReturn_FailureInstance(string message)
    {
        var result = Failure.Create(message);

        result.ShouldNotBeNull();
        result.ShouldBeOfType<Failure>();

        result.Success.ShouldBeFalse();

        result.Message.ShouldBe(message);
    }

    [Theory]
    [ClassData(typeof(FailureFactoryExceptionTestData))]
    public void FailureFactory_ShouldReturn_FailureInstance_ForException(Exception ex)
    {
        var result = Failure.Create(ex);

        result.ShouldNotBeNull();
        result.ShouldBeOfType<Failure>();

        result.Success.ShouldBeFalse();

        result.Message.ShouldBe(ex.Message);
    }

    internal class SuccessFactoryTestData : TheoryData<string, string>
    {
        public SuccessFactoryTestData()
        {
            Add("Data", "Message");
            Add("test.txt", "File found");
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

    internal class FailureFactoryExceptionTestData : TheoryData<Exception>
    {
        public FailureFactoryExceptionTestData()
        {
            Add(new Exception("Message"));
            Add(new Exception("File not found"));
        }
    }
}
