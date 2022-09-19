using System.Net.WebSockets;
using System.Reactive.Linq;
using Websocket.Client;

namespace GameServer.MainShardProxy;

public class SlaveServer : IGameSessionServer, IDisposable
{
    WebsocketClient _connection;
    Uri _uri;

    public event EventHandler<InitializeData>? OnInitialize;

    public SlaveServer(Uri uri)
    {
        _uri = uri;
        _connection = new WebsocketClient(uri);
        _connection.MessageReceived
            .Where(msg => msg.MessageType == WebSocketMessageType.Text)
            .Select(msg => Observable.FromAsync(() => OnMessageAsync(msg.Text)))
            .Subscribe();
        _connection.MessageReceived
            .Where(msg => msg.MessageType == WebSocketMessageType.Binary)
            .Select(msg => Observable.FromAsync(() => OnBinaryAsync(msg.Binary)))
            .Subscribe();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    public Task ConnectAsync()
    {
        _connection.Start();
        return Task.CompletedTask;
    }

    private Task OnMessageAsync(string text)
    {
        if (text == "init")
        {
            OnInitialize?.Invoke(this, new InitializeData());
        }
        return Task.CompletedTask;
    }

    private Task OnBinaryAsync(byte[] bytes)
    {
        return Task.CompletedTask;
    }

    public Task GameEndAsync()
    {
        throw new NotImplementedException();
    }
}
