using Agones;
using Common.Network;
using Common.Packet.ClientToServer;
using Common.Packet.ServerToClient;
using GameServer.Game;
using GameServer.Network;

namespace GameServer;

public class AgonesSupportServer : IGameMainServer
{
    AgonesSDK _agones;
    CancellationTokenSource _cts;

    public AgonesSupportServer(AgonesSDK agones, CancellationTokenSource cts)
    {
        _agones = agones;
        _cts = cts;
    }

    public async Task RunMain()
    {
        var gs = await _agones.GetGameServerAsync();
        var port = gs.Status.Ports[0].Port_;
        Console.WriteLine($"Start SquareGame server: {port}");

        IGameRoom gameRoom = new GameRoom();
        ISocketHandler socketHandler = new SocketHandler();
        socketHandler.OnStart += async (_, e) =>
        {
            await _agones.ReadyAsync();
        };
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

        gameRoom.OnGameEnd += async (_, _) =>
        {
            await _agones.ShutDownAsync();
            _cts.Cancel();
        };

        _ = Task.Run(async () =>
        {
            var period = gs.Spec.Health.PeriodSeconds;
            while (true)
            {
                if (_cts.IsCancellationRequested)
                    return;
                await _agones.HealthAsync();
                await Task.Delay(TimeSpan.FromSeconds(period));
            }
        });

        await socketHandler.Run(port, _cts.Token);

        async Task Ping(ISocketEx client, CS_Ping pingPacket)
        {
            await client.SendMessageAsync(new SC_Pong { Value = pingPacket.Value });
        }
    }
}
