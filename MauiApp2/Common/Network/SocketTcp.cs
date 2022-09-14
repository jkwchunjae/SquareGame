using Common.Packet;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Text;

namespace Common.Network;

public class SocketTcp : ISocketEx
{
    NetworkStream _stream;
    TcpClient _client;

    private CircularArray _recvBuffer = new CircularArray(10000);
    private byte[] _messageLengthBuffer = new byte[sizeof(int)];
    private byte[] _sendBuffer = new byte[10000];

    private AsyncLock _sendLock = new AsyncLock();
    public bool Connected => _client?.Connected ?? false;

    public SocketTcp(TcpClient client)
    {
        _client = client;
        _stream = client.GetStream();
    }

    public void Close()
    {
        _stream.Close();
    }

    #region Receive
    public async Task<(int MessageLength, PacketBase? Message)> ReceiveMessageAsync(CancellationToken ct = default)
    {
        var messageLength = await ReadMessageLengthAsync(ct);
        if (messageLength == 0)
        {
            return (0, default);
        }

        var message = await ReadMessageAsync(messageLength, ct);

        var packet = JsonConvert.DeserializeObject<PacketBase>(message);

        return (messageLength, packet);
    }

    private async Task<int> ReadMessageLengthAsync(CancellationToken ct)
    {
        while (_recvBuffer.Length < sizeof(int))
        {
            await FillRecvBufferAsync(ct);
        }
        _recvBuffer.Read(_messageLengthBuffer);
        var length = BitConverter.ToInt32(_messageLengthBuffer);
        return length;
    }

    private async Task<string> ReadMessageAsync(int messageLength, CancellationToken ct)
    {
        while (_recvBuffer.Length < messageLength)
        {
            await FillRecvBufferAsync(ct);
        }
        var message = _recvBuffer.ReadString(messageLength);
        return message;
    }

    private async Task<int> FillRecvBufferAsync(CancellationToken ct)
    {
        var memory = _recvBuffer.GetWritableMemory();
        var count = await _stream.ReadAsync(memory, ct);
        _recvBuffer.ShiftEndIndex(count);

        return count;
    }
    #endregion

    #region Send
    public async Task SendMessageAsync(PacketBase message, CancellationToken ct = default)
    {
        var json = JsonConvert.SerializeObject(message);
        await SendTextAsync(json, ct);
    }

    private async Task SendTextAsync(string text, CancellationToken ct)
    {
        using (await _sendLock.LockAsync())
        {
            var byteLength = Encoding.UTF8.GetBytes(text, 0, text.Length, _sendBuffer, sizeof(int));
            _sendBuffer[0] = (byte)((byteLength >> 0) & 255);
            _sendBuffer[1] = (byte)((byteLength >> 8) & 255);
            _sendBuffer[2] = (byte)((byteLength >> 16) & 255);
            _sendBuffer[3] = (byte)((byteLength >> 24) & 255);

            await _stream.WriteAsync(new Memory<byte>(_sendBuffer, 0, byteLength + sizeof(int)), ct);
        }
    }
    #endregion
}
