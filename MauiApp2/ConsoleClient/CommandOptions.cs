using CommandLine;
using Newtonsoft.Json;

namespace ConsoleClient;

internal class CommandOptions
{
    [Option('i', "ip", Required = true, Default = "localhost")]
    public string IpAddress { get; set; }
    [Option('p', "port", Required = true, Default = 55300)]
    public int Port { get; set; }
    [Option('n', "name", Required = true)]
    public string UserName { get; set; }

}
