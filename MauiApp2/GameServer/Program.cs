using CommandLine;
using Common.Network;
using Common.Packet;
using Common.Packet.ClientToServer;
using Common.Packet.ServerToClient;
using GameServer;
using GameServer.Game;
using GameServer.MainShardProxy;
using GameServer.Network;

Console.WriteLine("Hello, World!");

Func<int, CancellationTokenSource, Task> main = async (port, cts) =>
{
    IGameRoom gameRoom = new GameRoom();
    ISocketHandler socketHandler = new SocketHandler();
    socketHandler.OnConnect += (_, e) =>
    {
        Console.WriteLine("Client connected !!");
    };
    socketHandler.OnMessage += async (_, e) =>
    {
        var (client, packetBase) = e;
        switch (packetBase)
        {
            case CS_Ping pingPacket: await Ping(client, pingPacket); break;
            case CS_Login loginPacket: await gameRoom.Login(client, loginPacket!.Name!); break;
            case CS_Pick pickPacket: await gameRoom.Pick(client, pickPacket.Color); break;
            default: break;
        }
    };

    gameRoom.OnGameEnd += (_, _) =>
    {
        cts.Cancel();
    };

    await socketHandler.Run(port, cts.Token);

    async Task Ping(ISocketEx client, CS_Ping pingPacket)
    {
        await client.SendMessageAsync(new SC_Pong { Value = pingPacket.Value });
    }
};

await Parser.Default.ParseArguments<CommandOptions>(args)
    .WithParsedAsync(async (cmd) =>
    {
        CancellationTokenSource gameEndTokenSource = new CancellationTokenSource();
        if (cmd.Standalone)
        {
            await main(cmd.Port, gameEndTokenSource);
        }
        else
        {
            SlaveServer gameSessionServer = new SlaveServer(new Uri(cmd.GameShardWebsocketServerUrl));
            gameSessionServer!.OnInitialize += async (_, initData) =>
            {
                await main(initData.Port, gameEndTokenSource);
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

