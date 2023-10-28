
using System.Text.Json;
using System.Text.RegularExpressions;
using Awarean.Redis.Domain.UnitTests;
using FluentAssertions;
using static Awarean.Redis.Domain.Constants;

namespace Awarean.Redis.Domain.Resp2.UnitTests;

public partial class WhenDesserializingIntegers
{
    [GeneratedRegex(@"^-?\d+$")]
    private static partial Regex OnlyIntegersRegex();

    [Theory]
    [MemberData(nameof(IntegerGenerator))]
    public void ValidIntegerShouldDesserialize(string respString)
    {
        var actualData = respString.Desserialize();
        var onlyIntegers = OnlyIntegersRegex();

        actualData.Should().MatchRegex(onlyIntegers);
    }

    [Theory]
    [MemberData(nameof(IntegerGenerator))]
    public void ValidIntegersShouldEndWithNumericDigits(string respString)
    {
        var actualData = respString.Desserialize();
        actualData.Should().Match(x => char.IsDigit(x.Last()));
    }

    [Theory]
    [MemberData(nameof(InvalidIntegerGenerator))]
    public void InvalidIntegerShouldFail(string respString)
    {
        var actualData = respString.Desserialize();

        actualData.Should().StartWith(ErrorSign);
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
        yield return new object[] { "0".ToWellFormedIntegerCommand() };
        yield return new object[] { "-1".ToWellFormedIntegerCommand() };
        yield return new object[] { "12345".ToWellFormedIntegerCommand() };
        yield return new object[] { "-100000000".ToWellFormedIntegerCommand() };
        yield return new object[] { "100000000".ToWellFormedIntegerCommand() };
    }

    public static IEnumerable<object[]> InvalidIntegerGenerator()
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