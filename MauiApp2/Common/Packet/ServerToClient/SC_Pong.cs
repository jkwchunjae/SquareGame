namespace Common.Packet.ServerToClient;

public class SC_Pong : PacketBase
{
    public int Value { get; set; }

    public SC_Pong()
    {
        Type = PacketType.SC_Pong;
    }
}
