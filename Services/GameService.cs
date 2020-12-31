using System.Threading.Tasks;
using Blazor2048.Models;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

namespace Blazor2048.Services
{
    public class GameService : IGameService
    {
        private readonly ILocalStorageService _storageService;

        public GameService(ILocalStorageService storageService)
        {
            _storageService = storageService;
            var unused = InitializeAsync();
        }

        public ElementReference? Container { get; set; }
        public Game? Game { get; private set; }
        public int BestScore { get; private set; }

        public bool IsGameStarted => Game is not null;

        public async Task NewGameAsync()
        {
            await DeleteGameAsync();
            Game = new();
        }

        public async Task MoveAsync(Direction direction)
        {
            if (!IsGameStarted)
                return;

            if (!Game!.Move(direction))
                return;

            if (Game.Over)
                await DeleteGameAsync();
            else
                await SaveGameAsync();

            if (Game.Score > BestScore)
            {
                BestScore = Game.Score;
                await _storageService.SetItemAsync("bestScore", BestScore);
            }
        }

        public async Task UndoAsync()
        {
            if (!IsGameStarted)
                return;

            Game!.Undo();
            await SaveGameAsync();
        }

        public async Task<bool> LoadGameAsync()
        {
            var str = await _storageService.GetItemAsStringAsync("game");

            if (str is null)
                return false;

            var parts = str.Split(",");
            if (parts.Length != Grid.Size * Grid.Size + 1)
                return false;

            if (!int.TryParse(parts[Grid.Size * Grid.Size], out var score))
                return false;

            var values = new int?[Grid.Size * Grid.Size];
            for (var i = 0; i < values.Length; i++)
            {
                var part = parts[i];

                if (string.IsNullOrWhiteSpace(part))
                {
                    values[i] = null;
                    continue;
                }

                if (!int.TryParse(part, out var tileValue))
                    return false;

                values[i] = tileValue;
            }

            Game = new(score, values);
            return true;
        }

        public async Task FocusAsync()
        {
            if (!Container.HasValue)
                return;

            await Container.Value.FocusAsync();
        }

        private async Task InitializeAsync()
        {
            var item = await _storageService.GetItemAsStringAsync("bestScore");

            if (item is null)
                return;

            if (int.TryParse(item, out var bestScore))
                BestScore = bestScore;
        }

        public async Task SaveGameAsync()
        {
            if (!IsGameStarted)
                return;

            var str = string.Join(',', Game!.Grid.EnumerateCells()) + "," + Game.Score;
            await _storageService.SetItemAsync("game", str);
        }

        public async Task DeleteGameAsync() => await _storageService.RemoveItemAsync("game");
    }
}