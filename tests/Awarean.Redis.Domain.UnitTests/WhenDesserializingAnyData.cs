
using FluentAssertions;
using static Awarean.Redis.Domain.Constants;

namespace Awarean.Redis.Domain.UnitTests;

public class WhenDesserializingAnyData
{
    [Theory]
    [MemberData(nameof(InvalidStringGenerator))]
    public void InvalidStringsShouldReturnError(string invalidString)
    {
        var actualData = invalidString.Desserialize();

        actualData.Should().StartWith(ErrorSign).And.EndWith(LineBreak);
    }

    public static IEnumerable<object[]> InvalidStringGenerator()
    {
        yield return new object[] { "+OK" };
        yield return new object[] { "This is a valid string\r\n" };
        yield return new object[] { "\r\n" };
        yield return new object[] { "+" };
    }

}