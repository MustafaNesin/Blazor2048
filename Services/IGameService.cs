using System.Threading.Tasks;
using Blazor2048.Models;

namespace Blazor2048.Services
{
    public interface IGameService
    {
        int BestScore { get; }
        int Score { get; }
        int GridSize { get; }
        bool IsGamePaused { get; }
        bool IsGameWon { get; }
        bool IsGameOver { get; }
        bool CanGameUndo { get; }
        Task NewGameAsync();
        Task MoveAsync(Direction direction);
        Task UndoAsync();
        IReadOnlyCell? GetCell(int x, int y);
        void Resume();
        Task InitializeAsync();
    }
}