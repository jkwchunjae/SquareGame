namespace Common.Packet.ClientToServer;

public class CS_Ping : PacketBase
{
    public int Value { get; set; }

    public CS_Ping()
    {
        Type = PacketType.CS_Ping;
    }
}
