using Blazor2048.Models;

namespace Blazor2048.Services
{
    public class GameService : IGameService
    {
        public Game? Game { get; private set; }

        public bool IsGameStarted => Game is not null;
        public void NewGame() => Game = new();
    }
}