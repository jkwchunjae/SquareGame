using CommandLine;

namespace GameServer;

public class CommandOptions
{

    [Option('s', "standalone", Required = false, Default = false)]
    public bool Standalone { get; set; }
    [Option('p', "port", Required = false, Default = 55300)]
    public int Port { get; set; }
    [Option('u', "url", Required = false)]
    public string GameShardWebsocketServerUrl { get; set; }
}
