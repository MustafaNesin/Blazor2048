using System.Threading.Tasks;
using Blazor2048.Models;
using Microsoft.AspNetCore.Components;

namespace Blazor2048.Services
{
    public interface IGameService
    {
        int BestScore { get; }
        ElementReference? Container { get; set; }
        bool IsGameStarted { get; }
        Game? Game { get; }
        Task FocusAsync();
        Task<bool> LoadGameAsync();
        Task MoveAsync(Direction direction);
        Task NewGameAsync();
        Task UndoAsync();
    }
}