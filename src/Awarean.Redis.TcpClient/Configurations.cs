using System.Net.Sockets;
using System.Text.Json;

public partial class Program
{
    static IConfiguration GetConfiguration()
    {
        var environmentName = Environment.GetEnvironmentVariable("ENVIRONMENT_NAME");
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        return configuration;
    }

    static Dictionary<string, Func<string[], string>> GetCommands()
        => new Dictionary<string, Func<string[], string>>()
        {
            {"PING", (_) => "PONG"},
            {"ECHO", (args) => string.Join(" ", args) },
            {"SET", (args) => JsonSerializer.Serialize(new NotImplementedException()) }
        };

    static TcpClient GetTcpClient(IConfiguration configuration)
    {
        var hostAddress = configuration.GetValue<string>("hostAddress") ?? throw new ArgumentNullException("hostAddress");
        var port = configuration.GetValue<int?>("port") ?? throw new ArgumentNullException("port");
        var client = new TcpClient(hostAddress, port);
        return client;
    }
}