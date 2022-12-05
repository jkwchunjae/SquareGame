using System.Net;

namespace ConsoleClient;

internal class InitRequest
{
    public IPAddress IpAddress { get; set; }
    public int Port { get; set; }
    public string UserName { get; set; }
}
