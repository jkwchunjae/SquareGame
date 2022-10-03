using BitterClient;
using BitterClient.Model;
using JkwExtensions;
using MauiApp2.Game;
using Microsoft.AspNetCore.Components;
using System.Net;

namespace MauiApp2.Components;

public partial class ServerConnection
{
    [Inject] IGameService GameService { get; set; }
    IBitterUserLobby BitterUserLobby { get; set; }

    private string IpAddress = "localhost";
    private string Port = "55300";
    private string Name = "경원";

    private async Task Login()
    {
        var ip = IpAddress == "localhost" ? IPAddress.Loopback : IPAddress.Parse(IpAddress);
        // await GameService.Login(ip, Port.ToInt(), Name);
        BitterUserLobby = new BitterUserLobby($"http://{ip}:{Port}");
        await BitterUserLobby.AuthenticateAsync(new BitterAuthRequest(new UserId(Name)));
    }
}
