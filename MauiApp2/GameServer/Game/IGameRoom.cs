using Common.Network;

namespace GameServer.Game;

internal interface IGameRoom
{
    Task OnLogin(ISocketEx socket, string playerName);
    Task OnPick(ISocketEx socket, char color);
    Task OnDisconnect(ISocketEx socket);
}
