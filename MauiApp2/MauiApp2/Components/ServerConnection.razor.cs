using BitterClient;
using BitterClient.Model;
using JkwExtensions;
using MauiApp2.Game;
using Microsoft.AspNetCore.Components;
using System.Net;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MauiApp2.Components;

public partial class ServerConnection
{
    [Inject] IGameService GameService { get; set; }
    IBitterUserLobby BitterUserLobby { get; set; } = null;

    Timer _matchStatusTimer = null;

    private string BitterLobbyIpAddress = "localhost";
    private string BitterLobbyPort = "10080";
    private string Name = "경원";

    private string GameServerIpAddress = "localhost";
    private string GameServerPort = "55300";
    private bool _gameServerIsReady = false;

    private string _authToken = null;
    private bool IsLogin => !string.IsNullOrEmpty(_authToken);
    private string UserId;

    protected override void OnInitialized()
    {
        _matchStatusTimer = new Timer();
        _matchStatusTimer.Elapsed += async (s, e) => await CheckMatchStatus();
        _matchStatusTimer.Interval = TimeSpan.FromSeconds(1).TotalMilliseconds;
    }

    private async Task Login()
    {
        var ip = BitterLobbyIpAddress == "localhost" ? IPAddress.Loopback : IPAddress.Parse(BitterLobbyIpAddress);
        // await GameService.Login(ip, Port.ToInt(), Name);
        BitterUserLobby = new BitterUserLobby($"http://{ip}:{BitterLobbyPort}");
        var res = await BitterUserLobby.AuthenticateAsync(new BitterAuthRequest(new UserId(Name)));
        _authToken = res.Token;

        await UpdateUserInfo();
    }

    private async Task UpdateUserInfo()
    {
        var res = await BitterUserLobby.GetUserProfileAsync(new GetUserProfileRequest());
        UserId = res.UserId.Id;
    }

    private async Task RequestMatch()
    {
        _gameServerIsReady = false;
        await BitterUserLobby.MatchAsync(new());

        if (_matchStatusTimer != null)
        {
            _matchStatusTimer.Stop();
        }

        _matchStatusTimer.Start();
    }

    private async Task CheckMatchStatus()
    {
        MatchStatusRequest request = _gameServerIsReady ? new MatchStatusRequest
        {
            TestIpAddress = GameServerIpAddress,
            TestPort = GameServerPort.ToInt(),
        } : new();
        var status = await BitterUserLobby.GetMatchStatusAsync(request);
        if (!string.IsNullOrEmpty(status.IpAddress))
        {
            var ip = status.IpAddress == "localhost" ? IPAddress.Loopback : IPAddress.Parse(status.IpAddress);

            await GameService.Login(ip, status.Port, Name);
        }
    }

    private void GameServerIsReady()
    {
        _gameServerIsReady = true;
    }

    private async Task ConnectGameServer()
    {
        var ip = GameServerIpAddress == "localhost" ? IPAddress.Loopback : IPAddress.Parse(GameServerIpAddress);
        await GameService.Login(ip, GameServerPort.ToInt(), Name);
    }
}
