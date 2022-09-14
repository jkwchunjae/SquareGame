namespace ConsoleClient.Game;

internal class Board
{
    public int Size { get; init; }

    private string _board;
    private List<List<char>> _cells;
    private bool _isReverse;

    public Board(int size, string board, bool isReverse)
    {
        _isReverse = isReverse;
        Size = size;
        _board = isReverse ? new string(board.Reverse().ToArray()) : board;
        _cells = Enumerable.Range(0, size)
            .Select(row => _board.Substring(row * size, size).Select(c => c).ToList())
            .ToList();
    }

    public void Update(string board)
    {
        _board = _isReverse ? new string(board.Reverse().ToArray()) : board;
        _cells = Enumerable.Range(0, Size)
            .Select(row => _board.Substring(row * Size, Size).Select(c => c).ToList())
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
