using JkwExtensions;
using MauiApp2.Game;
using Microsoft.AspNetCore.Components;
using System.Net;

namespace MauiApp2.Components;

public partial class ServerConnection
{
    [Inject] IGameService GameService { get; set; }

    private string IpAddress = "localhost";
    private string Port = "55190";
    private string Name = "경원";

    private async Task Login()
    {
        var ip = IpAddress == "localhost" ? IPAddress.Loopback : IPAddress.Parse(IpAddress);
        await GameService.Login(ip, Port.ToInt(), Name);
    }
}
