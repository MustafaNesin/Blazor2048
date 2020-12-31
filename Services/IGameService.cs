using Blazor2048.Models;

namespace Blazor2048.Services
{
    public interface IGameService
    {
        Game? Game { get; }
        bool IsGameStarted { get; }
        void NewGame();
    }
}