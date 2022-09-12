namespace GameServer.Game;

internal interface IBoard
{
    bool IsEnd { get; }
    void CreateBaord(int width);
    void ChangeColor(int row, int column, char color);
    string GetBoard();
    char GetColor(int row, int column);
    int GetArea(int row, int column);
}
