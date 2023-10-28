
using System.Text.Json;
using FluentAssertions;
using static Awarean.Redis.Domain.Constants;

namespace Awarean.Redis.Domain.UnitTests;

public class WhenDesserializingSimpleStrings
{
    [Theory][MemberData(nameof(RespStringGenerator))]
    public void ValidStringShouldDesserialize(string respString)
    {
        var actualData = RespDesserializer.Desserialize(respString);

        actualData.Should().NotStartWith(ErrorSign);
    }

    [Theory][MemberData(nameof(RespStringAndResponseGenerator))]
    public void ValidStringShouldReturnValue(string respString, string expected)
    {
        var actualData = respString.Desserialize();

        actualData.Should().Be(expected);
    }

    public static IEnumerable<object[]> RespStringGenerator()
    {
        yield return new object[] { "OK".ToWellFormedSimpleStringCommand() };
        yield return new object[] { "This is a valid string".ToWellFormedSimpleStringCommand() };
        yield return new object[] { "".ToWellFormedSimpleStringCommand() };
        yield return new object[] { 
            JsonSerializer.Serialize(new { id = 1, name = "batman", anything = new[] { 1, 2, 3 } })
                .ToWellFormedSimpleStringCommand() 
        };
    }

    public static IEnumerable<object[]> RespStringAndResponseGenerator()
    {
        foreach (var respStringArray in RespStringGenerator())
        {
            var respString = respStringArray[0].ToString();
            var expectedString = respString.Replace(SimpleStringCommand.ToString(), Empty).Replace(LineBreak, Empty);
            yield return new object[] { respString, expectedString };
        }
    }
}
