using Common.Network;

namespace GameServer.Game;

internal interface IGameRoom
{
    Task Login(ISocketEx socket, string playerName);
    Task Pick(ISocketEx socket, char color);
    Task OnDisconnect(ISocketEx socket);

    event EventHandler OnGameStart;
    event EventHandler OnGameEnd;
}
