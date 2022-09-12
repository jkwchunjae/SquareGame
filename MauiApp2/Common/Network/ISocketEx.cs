using Common.Packet;

public interface ISocketEx
{
    void Close();
    Task<(int MessageLength, PacketBase? Message)> ReceiveMessageAsync(CancellationToken ct = default);
    Task SendMessageAsync(PacketBase message, CancellationToken ct = default);
}
