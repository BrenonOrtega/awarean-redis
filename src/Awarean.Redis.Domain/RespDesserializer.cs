namespace Awarean.Redis.Domain;
using static Constants;

public static class RespDesserializer
{
    private static readonly Dictionary<char, Func<string, string>> desserializers = new()
    {
        [SimpleStringCommand] = GetFormatter(SimpleStringCommand),
        [IntegerCommand] = GetFormatter(IntegerCommand),
    };

    private static Func<string, string> GetFormatter(char @char) 
        => (string respString) => respString
            .Replace(@char.ToString(), Empty)
            .Replace(LineBreak, Empty);

    public static string Desserialize(this string respString)
    {
        if (string.IsNullOrEmpty(respString))
            return GenerateErrorMessage("Supplied data did not have the proper format");

        if (respString.EndsWith(LineBreak) is false)
            return GenerateErrorMessage("The data supplied did not have the correct end of line.");

        char command = respString[0];

        var exists = desserializers.TryGetValue(command, out var formatter);

        if (exists is false)
            return GenerateErrorMessage("The data supplied did not have a data type indicator at the start");

        return formatter.Invoke(respString);
    }

    private static string GenerateErrorMessage(string errorMessage)
        => $"{ErrorSign}{errorMessage}{LineBreak}";
}
