using Common.Network;
using Common.Packet.ClientToServer;
using Common.Packet.ServerToClient;
using GameServer.Game;
using GameServer.Network;

namespace GameServer;

public class StandaloneServer : IGameMainServer
{
    private readonly int _port;
    private readonly CancellationTokenSource _cts;

    public StandaloneServer(int port, CancellationTokenSource cts)
    {
        _port = port;
        _cts = cts;
    }

    public async Task RunMain()
    {
        Console.WriteLine($"Start SquareGame server: {_port}");

        IGameRoom gameRoom = new GameRoom();
        ISocketHandler socketHandler = new SocketHandler();
        socketHandler.OnStart += (_, e) =>
        {
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

        gameRoom.OnGameEnd += (_, _) =>
        {
            _cts.Cancel();
        };

        await socketHandler.Run(_port, _cts.Token);

        async Task Ping(ISocketEx client, CS_Ping pingPacket)
        {
            await client.SendMessageAsync(new SC_Pong { Value = pingPacket.Value });
        }
    }
}


