using Common.Network;
using Common.Packet;

namespace GameServer.Network;

internal interface ISocketHandler
{
    event EventHandler OnStart;
    event EventHandler<ISocketEx> OnConnect;
    event EventHandler<ISocketEx> OnDisconnect;
    event EventHandler<(ISocketEx, PacketBase?)> OnMessage;
    Task Run(int port, CancellationToken cancellationToken);
}
