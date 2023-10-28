
using System.Text.Json;
using FluentAssertions;
using static Awarean.Redis.Domain.Constants;

namespace Awarean.Redis.Domain.UnitTests;

public class WhenDesserializingIntegers
{
    [Theory]
    [MemberData(nameof(IntegerGenerator))]
    public void ValidIntegerShouldDesserialize(string respString)
    {
        var actualData = RespDesserializer.Desserialize(respString);

        actualData.Should().NotStartWith(ErrorSign);
    }

    [Theory]
    [MemberData(nameof(IntegerAndResponseGenerator))]
    public void ValidStringShouldReturnValue(string respString, string expected)
    {
        var actualData = respString.Desserialize();

        actualData.Should().Be(expected);
    }

    public static IEnumerable<object[]> IntegerGenerator()
    {
        yield return new object[] { "OK".ToWellFormedIntegerCommand() };
        yield return new object[] { "This is a valid string".ToWellFormedIntegerCommand() };
        yield return new object[] { "".ToWellFormedIntegerCommand() };
        yield return new object[] 
        { 
            JsonSerializer.Serialize(new { id = 1, name = "batman", anything = anyArray })
                .ToWellFormedIntegerCommand() 
        };
    }

    private static readonly int[] anyArray = new[] { 1, 2, 3 };

    public static IEnumerable<object[]> IntegerAndResponseGenerator()
    {
        foreach (var respStringArray in IntegerGenerator())
        {
            var respString = respStringArray[0].ToString();
            var expectedString = respString.Replace(IntegerCommand.ToString(), Empty).Replace(LineBreak, Empty);
            yield return new object[] { respString, expectedString };
        }
    }
}