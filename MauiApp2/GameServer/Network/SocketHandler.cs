using System.Net.Sockets;
using System.Net;
using Common.Packet;
using Common.Network;

namespace GameServer.Network;

internal class SocketHandler : ISocketHandler
{
    public event EventHandler<ISocketEx>? OnConnect;
    public event EventHandler<ISocketEx>? OnDisconnect;
    public event EventHandler<(ISocketEx, PacketBase?)>? OnMessage;

    public async Task Run(int port)
    {
        await StartListening(port);
    }

    private async Task StartListening(int port)
    {
        try
        {
            IPAddress ipAddress = IPAddress.Any;
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            TcpListener server = new TcpListener(ipAddress, port);
            server.Start();

            while (true)
            {
                Console.WriteLine("Waiting for a connection....");
                var client = await server.AcceptTcpClientAsync();
                ISocketEx socketEx = new SocketTcp(client);

                OnConnect?.Invoke(this, socketEx);

                Task.Run(async () => await HandleConnection(socketEx));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private async Task HandleConnection(ISocketEx client)
    {
        try
        {
            while (true)
            {
                var (receiveCount, packet) = await client.ReceiveMessageAsync();

                if (receiveCount == 0)
                {
                    Console.WriteLine("receive 0.");

                    OnDisconnect?.Invoke(this, client);
                    return;
                }

                if (packet == default)
                {
                    client.Close();
                    //await client.SendMessageAsync(new SC_System
                    //{
                    //    Data = "올바르지 않은 패킷입니다.",
                    //});
                }

                OnMessage?.Invoke(this, (client, packet));
            }
        }
        catch (SocketException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }
}

