using Common.Packet;
using ConsoleClient.Game;

namespace ConsoleClient;

internal class ViewModel
{
    object _lock = new object();

    public UserRole UserRole { get; set; }
    public bool IsMyTurn { get; set; }
    public Board? Board { get; set; }
    public string? CurrentPlayerName { get; set; }
    public string? Player1Name { get; set; }
    public string? Player2Name { get; set; }
    public int Player1Score { get; set; }
    public int Player2Score { get; set; }
    public char Player1Color { get; set; }
    public char Player2Color { get; set; }

    public event EventHandler WaitPickColor;

    public ViewModel()
    {
    }

    public void Print()
    {
        lock (_lock)
        {
            Console.SetCursorPosition(0, 0);

            if (UserRole == UserRole.Player)
            {
                Console.WriteLine("당신은 플레이어입니다.               ");
                if (IsMyTurn)
                {
                    Console.WriteLine("당신 차례입니다.               ");
                }
                else
                {
                    Console.WriteLine("상대방 차례입니다.              ");
                }
            }
            else
            {
                Console.WriteLine("당신은 관전자입니다.             ");
                Console.WriteLine($"{CurrentPlayerName} 차례입니다.");
            }

            if (Board != null)
            {
                Console.WriteLine("                                     ");
                Console.WriteLine($"{Player1Name}   {Player1Score} : {Player2Score}   {Player2Name}");
                Console.WriteLine("                                     ");

                var size = Board.Size;
                for (var row = 0; row < size; row++)
                {
                    for (var column = 0; column < size; column++)
                    {
                        var color = Board.GetColor(row, column);
                        Console.BackgroundColor = GetConsoleColor(color);
                        Console.ForegroundColor = GetConsoleColor(color);
                        Console.Write($"{color}{color}");
                    }
                    Console.WriteLine();
                }
                Console.ResetColor();
            }

            if (IsMyTurn)
            {
                Console.WriteLine("                                                        ");
                var colors = new[] { 'A', 'B', 'C', 'D', 'E', 'F' };
                foreach (var color in colors)
                {
                    Console.BackgroundColor = GetConsoleColor(color);
                    Console.ForegroundColor = GetConsoleColor(color);
                    Console.Write($"{color}{color}");
                    Console.ResetColor();
                    Console.Write($" {color}  ");
                }
                Console.WriteLine("                                                        ");
                WaitPickColor?.Invoke(this, null);
            }
            else
            {
                Console.WriteLine("                                                        ");
                Console.WriteLine("                                                        ");
                Console.WriteLine("                                                        ");
            }
        }
    }

    public ConsoleColor GetConsoleColor(char color)
    {
        switch (color)
        {
            case 'A': return ConsoleColor.Magenta;
            case 'B': return ConsoleColor.DarkYellow;
            case 'C': return ConsoleColor.Cyan;
            case 'D': return ConsoleColor.Gray;
            case 'E': return ConsoleColor.Green;
            case 'F': return ConsoleColor.DarkBlue;
        }
        return ConsoleColor.White;
    }
}
