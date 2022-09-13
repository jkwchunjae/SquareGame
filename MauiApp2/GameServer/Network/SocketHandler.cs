using System.Net.Sockets;
using System.Net;
using Common.Packet;

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
        IPAddress ipAddress = IPAddress.Any;
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

        Socket listener = new Socket(ipAddress.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);

        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(100);

            while (true)
            {
                Console.WriteLine("Waiting for a connection....");
                var socket = await listener.AcceptAsync();
                var socketEx = new SocketEx(socket);

                OnConnect?.Invoke(this, socketEx);

                Task.Run(async () => await HandleConnection(new SocketEx(socket)));
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

