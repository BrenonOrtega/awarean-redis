using System.Text.RegularExpressions;
using static Awarean.Redis.Domain.Constants;

namespace Awarean.Redis.Domain.Resp2;

public static partial class RespDesserializer
{
    [GeneratedRegex(@"^-?\d+$")]
    private static partial Regex OnlyIntegersRegex();

    private static readonly Dictionary<char, Func<string, string>> desserializers = new()
    {
        [SimpleStringCommand] = GetFormatter(SimpleStringCommand),
        [IntegerCommand] = GetIntegerFormatter(),
    };

    private static Func<string, string> GetIntegerFormatter() => (s) =>
    {
        var formater = GetFormatter(IntegerCommand);
        var actualString = formater.Invoke(s);

        if (OnlyIntegersRegex().IsMatch(actualString))
            return actualString;

        return GenerateErrorMessage("WRONGTYPE", "An invalid data was provided for the indicated data type");
    };

    private static Func<string, string> GetFormatter(char @char) 
        => (string respString) => respString
            .Replace(@char.ToString(), Empty)
            .Replace(LineBreak, Empty);

    public static string Desserialize(this string respString)
    {
        if (string.IsNullOrEmpty(respString))
            return GenerateErrorMessage("ERR", "Supplied data did not have the proper format");

        if (respString.EndsWith(LineBreak) is false)
            return GenerateErrorMessage("ERR", "The data supplied did not have the correct end of line.");

        char command = respString[0];

        var exists = desserializers.TryGetValue(command, out var formatter);

        if (exists is false)
            return GenerateErrorMessage("ERR","The data supplied did not have a data type indicator at the start");

        return formatter.Invoke(respString);
    }

    private static string GenerateErrorMessage(string abbreviation, string errorMessage)
        => $"{ErrorSign}{abbreviation} {errorMessage}{LineBreak}";
}
