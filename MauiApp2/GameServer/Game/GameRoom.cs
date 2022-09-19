using Common;
using Common.Network;
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
    private User? _currentPlayer;

    private int _size = 15;
    private int _lastIndex => _size - 1;
    private IBoard _board;

    public event EventHandler OnGameStart;
    public event EventHandler OnGameEnd;

    public async Task OnDisconnect(ISocketEx socket)
    {
        using (await _lock.LockAsync())
        {
            _users.RemoveAll(user => user.Socket == socket);
        }
    }

    public async Task Login(ISocketEx socket, string playerName)
    {
        using (await _lock.LockAsync())
        {
            if (_users.Any(u => u.Socket == socket))
            {
                await SendBoard(socket, _currentPlayer);
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
                await SendBoard(user.Socket, _currentPlayer);
            }
        }
    }

    private async Task StartGame()
    {
        _board = new Board();
        _board.CreateBaord(_size);

        _currentPlayer = _player1;
        await _users
            .Select(async user =>
            {
                await SendBoard(user.Socket, _player1);
            })
            .WhenAll();

        OnGameStart?.Invoke(this, null);
    }

    private async Task SendBoard(ISocketEx client, User? current)
    {
        Console.WriteLine($"Turn: {current?.Name}");
        await client.SendMessageAsync(new SC_Board
        {
            Cells = _board.GetBoard(),
            Width = _size,
            CurrentPlayerName = current?.Name ?? string.Empty,
            Player1Name = _player1?.Name ?? string.Empty,
            Player1Color = _board.GetColor(0, 0),
            Player1Score = _board.GetArea(0, 0),
            Player2Name = _player2?.Name ?? string.Empty,
            Player2Color = _board.GetColor(_lastIndex, _lastIndex),
            Player2Score = _board.GetArea(_lastIndex, _lastIndex),
        });
    }

    public async Task Pick(ISocketEx socket, char color)
    {
        using (await _lock.LockAsync())
        {
            if (_board?.IsEnd ?? true)
                return;
            if (socket == null)
                return;
            if (_player1?.Socket != socket && _player2?.Socket != socket)
                return;

            if (color == _board.GetColor(0, 0) || color == _board.GetColor(_lastIndex, _lastIndex))
            {
                // await socket.SendMessageAsync(SC_Message: 본인의 색과 상대방의 색은 선택할 수 없습니다.);
                await SendBoard(socket, _player1?.Socket == socket ? _player1 : _player2);
                return;
            }

            var position = _player1?.Socket == socket ? (0, 0) : (_lastIndex, _lastIndex);
            _board.ChangeColor(position.Item1, position.Item2, color);

            await _users
                .Select(async user =>
                {
                    var nextUser = _player1?.Socket == socket ? _player2 : _player1;
                    _currentPlayer = nextUser;
                    await SendBoard(user.Socket, nextUser);
                })
                .WhenAll();

            if (_board.IsEnd)
            {
                await SendGameEnd();
            }
            else
            {
            }
        }
    }

    private async Task SendGameEnd()
    {
        var result = new SC_Result
        {
            Cells = _board.GetBoard(),
            Width = _size,
            Player1Name = _player1?.Name ?? string.Empty,
            Player1Color = _board.GetColor(0, 0),
            Player1Score = _board.GetArea(0, 0),
            Player2Name = _player2?.Name ?? string.Empty,
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

        OnGameEnd?.Invoke(this, null);
    }
}
