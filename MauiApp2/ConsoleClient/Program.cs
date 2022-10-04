﻿// See https://aka.ms/new-console-template for more information

using BitterClient;
using BitterClient.Model;
using ConsoleClient;
using ConsoleClient.Game;
using JkwExtensions;
using System.Net;

Console.WriteLine("Hello, World!");

Console.Write("Use lobby? (y:default, n)");
var useLobbyInput = Console.ReadLine();
bool useLobby = useLobbyInput == "y" || useLobbyInput == string.Empty ? true : false;

Console.Write("ip (localhost): ");
var ip = Console.ReadLine();
if (string.IsNullOrEmpty(ip))
    ip = "127.0.0.1";

var ipAddress = ip == "localhost" ? IPAddress.Loopback : IPAddress.Parse(ip);

var defaultPort = useLobby ? "10080" : "55300";
Console.Write($"port ({defaultPort}): ");
var port = Console.ReadLine();
if (string.IsNullOrEmpty(port))
    port = defaultPort;

string? name = string.Empty;

while (string.IsNullOrWhiteSpace(name))
{
    Console.Write("name: ");
    name = Console.ReadLine();
    if (string.IsNullOrEmpty(name))
        name = "jkw";
}

IGameService gameService = new GameService();
ViewModel view = new ViewModel();
bool gameRunning = true;
bool waitPickColor = false;

view.WaitPickColor += (_, __) =>
{
    waitPickColor = true;
};

gameService.OnYourRole += (s, e) =>
{
    view.UserRole = e.Role;
    view.Print();
};
gameService.OnBoard += (s, e) =>
{
    var isReverse = e.Player2Name == name;
    view.Board = new Board(e.Width, e.Cells, isReverse);
    view.IsMyTurn = e.CurrentPlayerName == name;
    view.CurrentPlayerName = e.CurrentPlayerName;
    view.Player1Name = e.Player1Name;
    view.Player2Name = e.Player2Name;
    view.Player1Score = e.Player1Score;
    view.Player2Score = e.Player2Score;
    view.Player1Color = e.Player1Color;
    view.Player2Color = e.Player2Color;
    view.Print();
};
gameService.OnResult += (s, e) =>
{
    gameRunning = false;
};

if (useLobby)
{
    var lobby = new BitterUserLobby($"http://{ip}:{port}");
    Console.WriteLine("로비에 로그인을 시도합니다.");
    await lobby.AuthenticateAsync(new BitterAuthRequest(new UserId(name)));

    Console.WriteLine("매치메이킹을 시도합니다.");
    await lobby.MatchAsync(new MatchRequest());
    while (true)
    {
        Console.WriteLine("현재 매치메이킹 상태를 확인합니다.");
        var res = await lobby.GetMatchStatusAsync(new MatchStatusRequest());
        if (!string.IsNullOrEmpty(res.IpAddress))
        {
            Console.WriteLine($"서버가 준비되었습니다. {res.IpAddress}:{res.Port}");
            await gameService.Login(IPAddress.Parse(res.IpAddress), res.Port, name!);
            break;
        }
        await Task.Delay(2000);
    }
}
else
{
    await gameService.Login(ipAddress, port.ToInt(), name!);
}

while (gameRunning)
{
    await Task.Delay(1000);

    if (waitPickColor)
    {
        char pickedColor = ' ';
        while (!"ABCDEF".Contains(pickedColor))
        {
            var picked = Console.ReadLine();
            if (picked?.Length == 1)
            {
                pickedColor = picked[0];
            }
        }
        if (view.IsMyTurn)
        {
            waitPickColor = false;
            await gameService.Pick(pickedColor);
        }
    }
}




