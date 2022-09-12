namespace MauiApp2.Game;

internal class Board
{
    public int Size { get; init; }

    private string _board;
    private List<List<char>> _cells;

    public Board(int size, string board)
    {
        Size = size;
        _board = board;
        _cells = Enumerable.Range(0, size)
            .Select(row => board.Substring(row * size, size).Select(c => c).ToList())
            .ToList();
    }

    public void Update(string board)
    {
        _board = board;
        _cells = Enumerable.Range(0, Size)
            .Select(row => board.Substring(row * Size, Size).Select(c => c).ToList())
            .ToList();
    }

    public char GetColor(int row, int column)
    {
        return _cells[row][column];
    }

    public override string ToString()
    {
        return _board;
    }
}
