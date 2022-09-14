using Common.Network;
using Common.Packet.ClientToServer;
using Common.Packet.ServerToClient;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Sockets;

namespace MauiApp2.Components;

public partial class PingPong
{
    ISocketEx connection;
    List<(DateTime Time, string Type)> TableData = new();
    private async Task Connect()
    {
        try
        {
            connection?.Close();

            IPEndPoint remoteEp = new IPEndPoint(IPAddress.Loopback, 55300);
            TcpClient client = new TcpClient();

            await client.ConnectAsync(remoteEp);

            connection = new SocketTcp(client);
            TableData.Add((DateTime.Now, "Connected"));
            StateHasChanged();

            await HandleReceiveAsync();
        }
        catch (Exception ex)
        {
            TableData.Add((DateTime.Now, ex.Message));
            connection?.Close();
            connection = null;
        }
    }

    private async Task Ping()
    {
        if (connection != null)
        {
            var pingPacket = new CS_Ping { Value = 0 };
            TableData.Add((DateTime.Now, pingPacket.Type.ToString()));
            StateHasChanged();

            try
            {
                await connection.SendMessageAsync(pingPacket);
            }
            catch (Exception ex)
            {
                TableData.Add((DateTime.Now, ex.Message));
            }
        }
    }

    private async Task HandleReceiveAsync()
    {
        while (true)
        {
            var (receiveCount, packet) = await connection.ReceiveMessageAsync();
            if (receiveCount == 0)
            {
                if (connection.Connected)
                {

                }
                else
                {

                }
                break;
            }

            switch (packet)
            {
                case SC_Pong pongPacket: await OnPongAsync(pongPacket); break;
                default:
                    break;
            }
        }
    }

    private ValueTask OnPongAsync(SC_Pong pongPacket)
    {
        TableData.Add((DateTime.Now, pongPacket.Type.ToString()));
        StateHasChanged();

        return ValueTask.CompletedTask;
    }
}
