using System.Threading.Tasks;
using Blazor2048.Models;

namespace Blazor2048.Services
{
    public interface IGameService
    {
        int BestScore { get; }
        bool IsGameStarted { get; }
        Game? Game { get; }
        Task<bool> LoadGameAsync();
        Task MoveAsync(Direction direction);
        Task NewGameAsync();
    }
}