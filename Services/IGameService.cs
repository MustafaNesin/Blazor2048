using System.Threading.Tasks;
using Blazor2048.Models;
using Microsoft.AspNetCore.Components;

namespace Blazor2048.Services
{
    public interface IGameService
    {
        int BestScore { get; }
        ElementReference? Container { set; }
        bool IsGameStarted { get; }
        bool IsTurkish { get; }
        Game? Game { get; }
        Task FocusAsync();
        Task<bool> LoadGameAsync();
        Task MoveAsync(Direction direction);
        Task NewGameAsync();
        Task UndoAsync();
    }
}