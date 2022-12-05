// See https://aka.ms/new-console-template for more information

using CommandLine;
using ConsoleClient;
using ConsoleClient.Game;
using JkwExtensions;
using System.Net;

Console.WriteLine("Hello, World!");

Func<InitRequest, Task> main = async (init) =>
{
    var ip = init.IpAddress;
    var port = init.Port;
    var name = init.UserName;

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

    await gameService.Login(ip, port, name!);

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
};


await Parser.Default.ParseArguments<CommandOptions>(args)
    .WithParsedAsync(async (cmd) =>
    {
        var init = new InitRequest
        {
            IpAddress = cmd.IpAddress == "localhost" ? IPAddress.Loopback : IPAddress.Parse(cmd.IpAddress),
            Port = cmd.Port,
            UserName = cmd.UserName,
        };
        await main(init);
    });


