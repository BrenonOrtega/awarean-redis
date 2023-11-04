using System.Net;
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

    static TcpListener GetTcpListener(IConfiguration configuration)
    {
        var hostAddress = configuration.GetValue<string>("hostAddress") ?? throw new ArgumentNullException("hostAddress");
        var port = configuration.GetValue<int?>("port") ?? throw new ArgumentNullException("port");
        int? maxConnectionsQuantity = configuration.GetValue<int?>("maxConnectionsQuantity");
        var endPoint = new IPEndPoint(IPAddress.Any, port);
        var listener = new TcpListener(endPoint);
        listener.Start(maxConnectionsQuantity ?? 10);
        return listener;
    }
}