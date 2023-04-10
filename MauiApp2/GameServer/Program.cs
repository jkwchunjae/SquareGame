using CommandLine;
using Common.Network;
using Common.Packet;
using Common.Packet.ClientToServer;
using Common.Packet.ServerToClient;
using GameServer;
using GameServer.Game;
using GameServer.MainShardProxy;
using GameServer.Network;
using Agones;

Console.WriteLine("Hello, World!");

var useAgones = Environment.GetEnvironmentVariable("AGONES")?.ToUpper() == "TRUE";

if (useAgones)
{
    var agonesSdk = new AgonesSDK();
    CancellationTokenSource gameEndTokenSource = new CancellationTokenSource();
    var server = new AgonesSupportServer(agonesSdk, gameEndTokenSource);
    await server.RunMain();
}
else
{
    await Parser.Default.ParseArguments<CommandOptions>(args)
        .WithParsedAsync(async (cmd) =>
        {
            CancellationTokenSource gameEndTokenSource = new CancellationTokenSource();
            if (cmd.Standalone)
            {
                var server = new StandaloneServer(cmd.Port, gameEndTokenSource);
                await server.RunMain();
            }
            else
            {
                SlaveServer gameSessionServer = new SlaveServer(new Uri(cmd.GameShardWebsocketServerUrl));
                gameSessionServer!.OnInitialize += async (_, initData) =>
                {
                    var server = new StandaloneServer(initData.Port, gameEndTokenSource);
                    await server.RunMain();
                };

                await gameSessionServer.ConnectAsync();

                while (true)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    if (gameEndTokenSource.IsCancellationRequested)
                    {
                        await gameSessionServer.GameEndAsync();
                        break;
                    }
                }
            }
        });
}


