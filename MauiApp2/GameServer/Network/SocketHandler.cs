using System.Net.Sockets;
using System.Net;
using Common.Packet;
using Common.Network;

namespace GameServer.Network;

internal class SocketHandler : ISocketHandler
{
    public event EventHandler? OnStart;
    public event EventHandler<ISocketEx>? OnConnect;
    public event EventHandler<ISocketEx>? OnDisconnect;
    public event EventHandler<(ISocketEx, PacketBase?)>? OnMessage;

    public async Task Run(int port, CancellationToken cancellationToken)
    {
        await StartListening(port, cancellationToken);
    }

    private async Task StartListening(int port, CancellationToken cancellationToken)
    {
        try
        {
            IPAddress ipAddress = IPAddress.Any;
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            TcpListener server = new TcpListener(ipAddress, port);
            server.Start();

            OnStart?.Invoke(this, new());

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;
                try
                {
                    Console.WriteLine("Waiting for a connection....");
                    var client = await server.AcceptTcpClientAsync(cancellationToken);

                    if (cancellationToken.IsCancellationRequested)
                        return;

                    ISocketEx socketEx = new SocketTcp(client);

                    OnConnect?.Invoke(this, socketEx);

                    _ = Task.Run(async () => await HandleConnection(socketEx, cancellationToken));
                }
                catch
                {
                    return;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private async Task HandleConnection(ISocketEx client, CancellationToken cancellationToken)
    {
        try
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                try
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
                catch
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    throw;
                }
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

