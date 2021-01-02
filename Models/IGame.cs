namespace Blazor2048.Models
{
    public interface IGame
    {
        int Score { get; }
        bool IsOver { get; }
        bool IsWon { get; }
        bool CanUndo { get; }
        IGrid Grid { get; }
        int WinningTileValue { get; }
        bool Move(Direction direction);
        bool Undo();
    }
}