using System.Drawing;

namespace GameServer.Game;

internal interface IGameRoom
{
    Task OnLogin(ISocketEx socket, string playerName);
    Task OnPick(ISocketEx socket, char color);
    Task OnDisconnect(ISocketEx socket);
}
