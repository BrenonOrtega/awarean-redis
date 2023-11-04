using System.Text;

args = new[] { "ECHO", "HELLO WORLD" };
var config = GetConfiguration();
using var client = GetTcpClient(config);
try
{
    using var stream = client.GetStream();

    var buffer = new byte[1_024];
    var commandString = Encoding.UTF8.GetBytes(string.Join(" ", args), buffer);

    await stream.WriteAsync(buffer);
    var responseBuffer = new byte[1_024];
    var response = await stream.ReadAsync(responseBuffer);
    Console.WriteLine(Encoding.UTF8.GetString(responseBuffer));
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    client.Close();
    client.Dispose();
}