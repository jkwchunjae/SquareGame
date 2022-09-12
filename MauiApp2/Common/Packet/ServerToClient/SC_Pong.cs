namespace Common.Packet.ServerToClient;

public class SC_Pong : PacketBase
{
    public int Value { get; set; }
    public SC_Pong()
    {
        Type = PacketType.SC_Pong;
    }
}

public class SC_YourRole : PacketBase
{
    public UserRole Role { get; set; }
    public SC_YourRole()
    {
        Type = PacketType.SC_YourRole;
    }
}

public class SC_Board : PacketBase
{
    public int Width { get; set; }
    public string? Cells { get; set; }
    public string? Player1Name { get; set; }
    public string? Player2Name { get; set; }
    public int Player1Score { get; set; }
    public int Player2Score { get; set; }
    public char Player1Color { get; set; }
    public char Player2Color { get; set; }
    public SC_Board()
    {
        Type = PacketType.SC_Board;
    }
}

public class SC_YourTurn : PacketBase
{
    public SC_YourTurn()
    {
        Type = PacketType.SC_YourTurn;
    }
}

public class SC_Result : PacketBase
{
    public int Width { get; set; }
    public string? Cells { get; set; }
    public string? Player1Name { get; set; }
    public string? Player2Name { get; set; }
    public int Player1Score { get; set; }
    public int Player2Score { get; set; }
    public char Player1Color { get; set; }
    public char Player2Color { get; set; }
    public string? WinnerName { get; set; }
    public SC_Result()
    {
        Type = PacketType.SC_Result;
    }
}
