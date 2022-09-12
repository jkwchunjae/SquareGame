using JkwExtensions;

namespace GameServer.Game;

internal class Board : IBoard
{
    const int ColorCount = 6;

    List<List<char>> _cells;
    object _lock = new object();

    public bool IsEnd
    {
        get
        {
            if (_cells == null)
                return true;

            var colors = _cells
                .SelectMany(color => color)
                .Distinct()
                .Count();

            return colors == 2;
        }
    }

    public void ChangeColor(int row, int column, char color)
    {
        lock (_lock)
        {
            List<(int Row, int Column)> reserved = new();
            var initColor = _cells[row][column];

            Queue<(int Row, int Column)> queue = new();
            queue.Enqueue((row, column));

            while (queue.Any())
            {
                var currentCell = queue.Dequeue();
                if (reserved.Contains(currentCell))
                    continue;
                reserved.Add(currentCell);

                Edges(currentCell)
                    .Where(x => _cells[x.Row][x.Column] == initColor)
                    .Where(x => !reserved.Contains(x))
                    .Where(x => !queue.Contains(x))
                    .ForEach(cell =>
                    {
                        queue.Enqueue(cell);
                    });
            }

            reserved.ForEach(cell =>
            {
                _cells[cell.Row][cell.Column] = color;
            });
        }

        List<(int Row, int Column)> Edges((int Row, int Column) currCell)
        {
            (int dx, int dy)[] dd = new[] { (-1, 0), (1, 0), (0, -1), (0, 1) };
            return dd
                .Select(d => (Row: currCell.Row + d.dx, Column: currCell.Column + d.dy))
                .Where(x => x.Row >= 0 && x.Row < _cells.Count)
                .Where(x => x.Column >= 0 && x.Column < _cells[0].Count)
                .ToList();
        }
    }

    public void CreateBaord(int width)
    {
        lock (_lock)
        {
            _cells = Enumerable.Range(0, width)
                .Select(_ => Enumerable.Range(0, width).Select(_ => (char)0).ToList())
            .ToList();

            for (var row = 0; row < width; row++)
            {
                for (var col = 0; col < width - row; col++)
                {
                    var index = StaticRandom.Next(ColorCount);
                    _cells[row][col] = (char)('A' + index);
                    _cells[width - row - 1][width - col - 1] = (char)('A' + (ColorCount - index - 1));
                }
            }
        }
    }

    public int GetArea(int row, int column)
    {
        lock (_lock)
        {
            List<(int Row, int Column)> reserved = new();
            var initColor = _cells[row][column];

            Queue<(int Row, int Column)> queue = new();
            queue.Enqueue((row, column));

            while (queue.Any())
            {
                var currentCell = queue.Dequeue();
                if (reserved.Contains(currentCell))
                    continue;
                reserved.Add(currentCell);

                Edges(currentCell)
                    .Where(x => _cells[x.Row][x.Column] == initColor)
                    .Where(x => !reserved.Contains(x))
                    .Where(x => !queue.Contains(x))
                    .ForEach(cell =>
                    {
                        queue.Enqueue(cell);
                    });
            }

            return reserved.Count;
        }

        List<(int Row, int Column)> Edges((int Row, int Column) currCell)
        {
            (int dx, int dy)[] dd = new[] { (-1, 0), (1, 0), (0, -1), (0, 1) };
            return dd
                .Select(d => (currCell.Row + d.dx, currCell.Column + d.dy))
                .Where(x => x.Item1 >= 0 && x.Item1 < _cells.Count)
                .Where(x => x.Item2 >= 0 && x.Item2 < _cells[0].Count)
                .ToList();
        }
    }

    public string GetBoard()
    {
        return ToString();
    }

    public char GetColor(int row, int column)
    {
        return _cells[row][column];
    }

    public override string ToString()
    {
        lock (_lock)
        {
            if (_cells == null)
                return string.Empty;

            return _cells
                .Select(row => new string(row.ToArray()))
                .StringJoin("");
        }
    }
}
