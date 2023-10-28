
using static Awarean.Redis.Domain.Constants;

namespace Awarean.Redis.Domain.UnitTests;

public static class TestExtensions
{
    public static string ToWellFormedIntegerCommand(this string @string) 
        => ToWellFormedRespCommand(@string, IntegerCommand);

    public static string ToWellFormedSimpleStringCommand(this string @string) 
        => ToWellFormedRespCommand(@string, SimpleStringCommand);

    public static string ToWellFormedRespCommand(this string @string, char commandSign) 
        => $"{commandSign}{@string} {LineBreak}";
}