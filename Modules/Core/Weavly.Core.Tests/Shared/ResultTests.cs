using Shouldly;
using Weavly.Core.Shared.Implementation;

namespace Weavly.Core.Tests.Shared;

public sealed class ResultTests
{
    [Theory]
    [ClassData(typeof(SuccessFactoryTestData))]
    public void SuccessFactory_ShouldReturn_SuccessInstance(string data)
    {
        var result = Result.Success(data);

        result.ShouldNotBeNull();

        result.Success.ShouldBeTrue();

        result.Data.ShouldNotBeNull();
        result.Data.ShouldBe(data);

        result.Message.ShouldBeNull();
    }

    [Theory]
    [ClassData(typeof(FailureFactoryTestData))]
    public void FailureFactory_ShouldReturn_FailureInstance(string message)
    {
        var result = Result.Failure(message);

        result.ShouldNotBeNull();
        result.ShouldBeOfType<Failure>();

        result.Success.ShouldBeFalse();

        result.Message.ShouldBe(message);
    }

    [Theory]
    [ClassData(typeof(FailureFactoryExceptionTestData))]
    public void FailureFactory_ShouldReturn_FailureInstance_ForException(Exception ex)
    {
        var result = Result.Failure(ex);

        result.ShouldNotBeNull();
        result.ShouldBeOfType<Failure>();

        result.Success.ShouldBeFalse();

        result.Message.ShouldBe(ex.Message);
    }

    public class SuccessFactoryTestData : TheoryData<string>
    {
        public SuccessFactoryTestData()
        {
            Add("Data");
            Add("test.txt");
        }
    }

    public class FailureFactoryTestData : TheoryData<string>
    {
        public FailureFactoryTestData()
        {
            Add("Message");
            Add("File not found");
        }
    }

    public class FailureFactoryExceptionTestData : TheoryData<Exception>
    {
        public FailureFactoryExceptionTestData()
        {
            Add(new Exception("Message"));
            Add(new Exception("File not found"));
        }
    }
}
