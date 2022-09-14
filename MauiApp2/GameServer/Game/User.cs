using Common.Network;

namespace GameServer.Game;

internal class User
{
    public ISocketEx Socket { get; init; }
    public string Name { get; init; }

    public User(ISocketEx socket, string name)
    {
        Socket = socket;
        Name = name;
    }
}
