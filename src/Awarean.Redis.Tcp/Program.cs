using System.Net.Sockets;
using System.Text;

var config = GetConfiguration();
var commands = GetCommands();
var listener = GetTcpListener(config);
listener.Start();

while (true)
{
    var socket = await listener.AcceptSocketAsync();
    try
    {
        listener.Server.Listen(config.GetValue<int?>("maxConnectionsNumber") ?? 10);
        string[] commandAndArgs = await ReceiveMessageAsync(socket);

        var command = commandAndArgs[0];
        var commandArgs = commandAndArgs.Where(x => x != command).ToArray();

        if (commands.TryGetValue(command, out Func<string[], string> commandProcessor))
        {
            var response = commandProcessor?.Invoke(commandArgs);
            var encodedResponse = Encoding.UTF8.GetBytes(response);
            await socket.SendAsync(encodedResponse);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        break;
    }
}

listener.Stop();
listener.Dispose();

static async Task<string[]> ReceiveMessageAsync(Socket socket)
{
    var index = 0;
    var buffer = new byte[1_024];
    var commandString = string.Empty;

    var received = await socket.ReceiveAsync(buffer);

    commandString = Encoding.UTF8.GetString(buffer);
    var commandAndArgs = commandString.Split(" ");
    return commandAndArgs;
}