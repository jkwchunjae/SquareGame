using Common.Packet;
using Common.Packet.ServerToClient;
using MauiApp2.Game;
using Microsoft.AspNetCore.Components;

namespace MauiApp2.Components;

public partial class GameBoard
{
    [Inject] IGameService GameService { get; set; }

    bool IsMyTurn = false;
    UserRole MyRole = UserRole.Spectator;
    Board Board;

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            GameService.OnYourRole += GameService_OnYourRole;
            GameService.OnYourTurn += GameService_OnYourTurn;
            GameService.OnBoard += GameService_OnBoard;
            GameService.OnResult += GameService_OnResult;
        }
        base.OnAfterRender(firstRender);
    }

    private void GameService_OnYourRole(object sender, SC_YourRole e)
    {
        Action action = () =>
        {
            MyRole = e.Role;
            StateHasChanged();
        };
        if (MainThread.IsMainThread)
        {
            action();
        }
        else
        {
            MainThread.BeginInvokeOnMainThread(action);
        }
    }

    private void GameService_OnYourTurn(object sender, SC_YourTurn e)
    {
        Action action = () =>
        {
            IsMyTurn = true;
            StateHasChanged();
        };
        if (MainThread.IsMainThread)
        {
            action();
        }
        else
        {
            MainThread.BeginInvokeOnMainThread(action);
        }
    }

    private void GameService_OnBoard(object sender, SC_Board e)
    {
        Action action = () =>
        {
            if (Board == null)
            {
                Board = new Board(e.Width, e.Cells);
            }
            else
            {
                Board.Update(e.Cells);
            }
            StateHasChanged();
        };
        if (MainThread.IsMainThread)
        {
            action();
        }
        else
        {
            MainThread.BeginInvokeOnMainThread(action);
        }
    }

    private void GameService_OnResult(object sender, SC_Result e)
    {
        throw new NotImplementedException();
    }

    private async Task Pick(char color)
    {
        IsMyTurn = false;
        StateHasChanged();
        await GameService.Pick(color);
    }
}
