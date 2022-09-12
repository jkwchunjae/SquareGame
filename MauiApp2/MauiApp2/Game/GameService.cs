using Common.Packet;
using Common.Packet.ClientToServer;
using Common.Packet.ServerToClient;
using System.Net;
using System.Net.Sockets;

namespace MauiApp2.Game;

public interface IGameService
{
    event EventHandler Disconnected;
    event EventHandler<SC_YourRole> OnYourRole;
    event EventHandler<SC_YourTurn> OnYourTurn;
    event EventHandler<SC_Board> OnBoard;
    event EventHandler<SC_Result> OnResult;
    Task Login(IPAddress ip, int port, string name);
    Task Pick(char color);
}

public class GameService : IGameService
{
    public event EventHandler Disconnected;
    public event EventHandler<SC_YourRole> OnYourRole;
    public event EventHandler<SC_YourTurn> OnYourTurn;
    public event EventHandler<SC_Board> OnBoard;
    public event EventHandler<SC_Result> OnResult;

    SocketEx _connection;
    UserRole _myRole = UserRole.Spectator;
    public async Task Login(IPAddress ip, int port, string name)
    {
        try
        {
            _connection?.Close();

            IPEndPoint remoteEp = new IPEndPoint(ip, 55190);

            Socket server = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            await server.ConnectAsync(remoteEp);
            _connection = new SocketEx(server);
            await _connection.SendMessageAsync(new CS_Login { Name = name });

            Task.Run(async () => await HandleReceiveAsync());
        }
        catch (Exception ex)
        {
            _connection?.Close();
            _connection = null;
            Disconnected?.Invoke(this, null);
        }
    }

    public async Task Pick(char color)
    {
        if (_myRole == UserRole.Player)
        {
            await _connection.SendMessageAsync(new CS_Pick { Color = color });
        }
    }

    private async Task HandleReceiveAsync()
    {
        while (true)
        {
            var (receiveCount, packet) = await _connection.ReceiveMessageAsync();
            if (receiveCount == 0)
            {
                if (_connection.Connected)
                {

                }
                else
                {

                }
                break;
            }

            switch (packet)
            {
                case SC_YourRole yourRolePacket: OnOnYourRole(yourRolePacket); break;
                case SC_YourTurn yourTurnPacket: OnYourTurn?.Invoke(this, yourTurnPacket); break;
                case SC_Board boardPacket: OnBoard?.Invoke(this, boardPacket); break;
                case SC_Result resultPacket: OnResult?.Invoke(this, resultPacket); break;
                default:
                    break;
            }
        }
    }

    void OnOnYourRole(SC_YourRole yourRolePacket)
    {
        _myRole = UserRole.Player;
        OnYourRole?.Invoke(this, yourRolePacket);
    }
}
