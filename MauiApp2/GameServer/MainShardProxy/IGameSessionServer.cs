namespace GameServer.MainShardProxy;

public class InitializeData
{
    public int Port { get; set; }
}

public interface IGameSessionServer
{
    event EventHandler<InitializeData> OnInitialize;
    Task GameEndAsync();
}
