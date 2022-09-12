namespace Common.Packet.ClientToServer;

public class CS_Ping : PacketBase
{
    public int Value { get; set; }

    public CS_Ping()
    {
        Type = PacketType.CS_Ping;
    }
}

public class CS_Login : PacketBase
{
    public string? Name { get; set; }
    public CS_Login()
    {
        Type = PacketType.CS_Login;
    }
}

public class CS_Pick : PacketBase
{
    public char Color { get; set; }
    public CS_Pick()
    {
        Type = PacketType.CS_Pick;
    }
}
