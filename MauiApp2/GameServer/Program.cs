using Common.Packet;
using Common.Packet.ClientToServer;
using Common.Packet.ServerToClient;
using GameServer.Game;
using GameServer.Network;

Console.WriteLine("Hello, World!");

IGameRoom gameRoom = new GameRoom();

ISocketHandler socketHandler = new SocketHandler();
socketHandler.OnMessage += async (_, e) =>
{
    var (client, packetBase) = e;
    switch (packetBase)
    {
        case CS_Ping pingPacket: await OnPing(client, pingPacket); break;
        case CS_Login loginPacket: await gameRoom.OnLogin(client, loginPacket!.Name!); break;
        case CS_Pick pickPacket: await gameRoom.OnPick(client, pickPacket.Color); break;
        default: break;
    }
};

await socketHandler.Run(55190);

async Task OnPing(ISocketEx client, CS_Ping pingPacket)
{
    await client.SendMessageAsync(new SC_Pong { Value = pingPacket.Value });
}
