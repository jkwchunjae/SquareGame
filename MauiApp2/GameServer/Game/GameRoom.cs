using Common;
using Common.Packet;
using Common.Packet.ServerToClient;
using JkwExtensions;

namespace GameServer.Game;

internal class GameRoom : IGameRoom
{
    private AsyncLock _lock = new AsyncLock();
    private List<User> _users = new();
    private User? _player1;
    private User? _player2;

    private int _size = 15;
    private int _lastIndex => _size - 1;
    private IBoard _board;

    public async Task OnDisconnect(ISocketEx socket)
    {
        using (await _lock.LockAsync())
        {
            _users.RemoveAll(user => user.Socket == socket);
        }
    }

    public async Task OnLogin(ISocketEx socket, string playerName)
    {
        using (await _lock.LockAsync())
        {
            if (_users.Any(u => u.Socket == socket))
            {
                await SendBoard(socket);
            }

            var user = new User(socket, playerName);
            _users.Add(user);
            if (_player1 == null)
            {
                _player1 = user;
                await user.Socket.SendMessageAsync(new SC_YourRole { Role = UserRole.Player });
            }
            else if (_player2 == null)
            {
                _player2 = user;
                await user.Socket.SendMessageAsync(new SC_YourRole { Role = UserRole.Player });

                await StartGame();
            }
            else
            {
                await user.Socket.SendMessageAsync(new SC_YourRole { Role = UserRole.Spectator });
                await SendBoard(user.Socket);
            }
        }
    }

    private async Task StartGame()
    {
        _board = new Board();
        _board.CreateBaord(_size);

        await _users
            .Select(async user =>
            {
                await SendBoard(user.Socket);
            })
            .WhenAll();

        await _player1!.Socket.SendMessageAsync(new SC_YourTurn());
    }

    private async Task SendBoard(ISocketEx client)
    {
        await client.SendMessageAsync(new SC_Board
        {
            Cells = _board.GetBoard(),
            Width = _size,
            Player1Name = _player1!.Name,
            Player1Color = _board.GetColor(0, 0),
            Player1Score = _board.GetArea(0, 0),
            Player2Name = _player2!.Name,
            Player2Color = _board.GetColor(_lastIndex, _lastIndex),
            Player2Score = _board.GetArea(_lastIndex, _lastIndex),
        });
    }

    public async Task OnPick(ISocketEx socket, char color)
    {
        using (await _lock.LockAsync())
        {
            if (socket == null)
                return;
            if (_player1?.Socket == socket || _player2?.Socket == socket)
                return;

            var position = _player1?.Socket == socket ? (0, 0) : (_lastIndex, _lastIndex);
            _board.ChangeColor(position.Item1, position.Item2, color);

            await _users
                .Select(async user =>
                {
                    await SendBoard(user.Socket);
                })
                .WhenAll();

            if (_board.IsEnd)
            {
                await SendGameEnd();
            }
            else
            {
                var nextUser = _player1?.Socket == socket ? _player2 : _player1;
                await nextUser!.Socket.SendMessageAsync(new SC_YourTurn());
            }
        }
    }

    private async Task SendGameEnd()
    {
        var result = new SC_Result
        {
            Cells = _board.GetBoard(),
            Width = _size,
            Player1Name = _player1!.Name,
            Player1Color = _board.GetColor(0, 0),
            Player1Score = _board.GetArea(0, 0),
            Player2Name = _player2!.Name,
            Player2Color = _board.GetColor(_lastIndex, _lastIndex),
            Player2Score = _board.GetArea(_lastIndex, _lastIndex),
        };
        result.WinnerName = result.Player1Score > result.Player2Score ? result.Player1Name : result.Player2Name;

        await _users
            .Select(async user =>
            {
                await user.Socket.SendMessageAsync(result);
            })
            .WhenAll();

        _users.ForEach(user =>
        {
            user.Socket.Close();
        });

        _player1 = null;
        _player2 = null;
        _users = new();
    }
}
