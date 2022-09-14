using Common.Packet;

namespace Common.Network;

public interface ISocketEx
{
    bool Connected { get; }
    void Close();
    Task<(int MessageLength, PacketBase? Message)> ReceiveMessageAsync(CancellationToken ct = default);
    Task SendMessageAsync(PacketBase message, CancellationToken ct = default);
}
