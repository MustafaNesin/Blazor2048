using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazor2048.Models;
using Blazor2048.Properties;
using Blazored.LocalStorage;
using Environment = Blazor2048.Properties.Environment;

namespace Blazor2048.Services
{
    public class GameService : IGameService
    {
        public const int DefaultWinningTileValue = 2048;
        private const int DefaultGridSize = 4;
        private const int DefaultStartTiles = 2;
        private readonly ILocalStorageService _storageService;
        private IGame _game = null!;

        public GameService(ILocalStorageService storageService) => _storageService = storageService;

        public int BestScore { get; private set; }
        public int Score => _game.Score;
        public int GridSize => _game.Grid.Size;
        public bool IsGamePaused { get; private set; }
        public bool IsGameWon => _game.IsWon;
        public bool IsGameOver => _game.IsOver;
        public bool CanGameUndo => _game.CanUndo;

        public async Task NewGameAsync()
        {
            IsGamePaused = true;
            await DeleteGameAsync();
            _game = new Game(DefaultGridSize, DefaultStartTiles, DefaultWinningTileValue);
            IsGamePaused = false;
        }

        public async Task MoveAsync(Direction direction)
        {
            if (IsGamePaused)
                return;

            IsGamePaused = true;
            var wasWon = _game.IsWon;

            if (!_game.Move(direction))
            {
                IsGamePaused = false;
                return;
            }

            if (_game.IsOver)
                await DeleteGameAsync();
            else
                await SaveGameAsync();

            if (_game.Score > BestScore)
            {
                BestScore = _game.Score;
                await _storageService.SetItemAsync(Environment.BestScoreStorageItemKey, BestScore);
            }

            if (!_game.IsOver && wasWon == _game.IsWon)
                IsGamePaused = false;
        }

        public async Task UndoAsync()
        {
            if (IsGamePaused && !IsGameOver)
                return;

            IsGamePaused = true;

            if (_game.Undo())
                await SaveGameAsync();

            IsGamePaused = false;
        }

        public IReadOnlyCell GetCell(int x, int y)
        {
            if (y < 0 || y >= GridSize)
                throw new ArgumentException(Localization.ArgumentExceptionOutOfGridRows, nameof(y));

            if (x < 0 || x >= GridSize)
                throw new ArgumentException(Localization.ArgumentExceptionOutOfGridColumns, nameof(x));

            return (IReadOnlyCell)_game.Grid[x, y];
        }

        public void Resume()
        {
            if (_game.IsOver)
                return;

            IsGamePaused = false;
        }

        public async Task InitializeAsync()
        {
            if (!await LoadGameAsync())
                _game = new Game(DefaultGridSize, DefaultStartTiles, DefaultWinningTileValue);

            var item = await _storageService.GetItemAsStringAsync(Environment.BestScoreStorageItemKey);

            if (item is null)
                return;

            if (int.TryParse(item, out var bestScore))
                BestScore = bestScore;
        }

        private async Task<bool> LoadGameAsync()
        {
            var str = await _storageService.GetItemAsStringAsync(Environment.LastGameStorageItemKey);

            if (str is null)
                return false;

            try
            {
                var winningTileValue = DefaultWinningTileValue;
                var parts = str.Split(",");
                var lastPart = parts[^1];

                if (lastPart.EndsWith('!')) // If format of the save is newer
                {
                    winningTileValue = int.Parse(lastPart[..^1]);
                    lastPart = parts[^2];
                    Array.Resize(ref parts, parts.Length - 2);
                }
                else
                    Array.Resize(ref parts, parts.Length - 1);

                var score = int.Parse(lastPart);
                var tileValues = parts
                    .Select(part => part.Length == 0 ? (int?)null : int.Parse(part))
                    .ToList();

                _game = new Game(winningTileValue, score, tileValues);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task SaveGameAsync()
        {
            var stringBuilder = new StringBuilder(32);

            foreach (var cell in _game.Grid.EnumerateCells())
                stringBuilder.AppendFormat("{0},", cell.TileValue);

            stringBuilder.AppendFormat("{0},", _game.Score);
            stringBuilder.AppendFormat("{0}!", _game.WinningTileValue);

            await _storageService.SetItemAsync(Environment.LastGameStorageItemKey, stringBuilder.ToString());
        }

        private async Task DeleteGameAsync() =>
            await _storageService.RemoveItemAsync(Environment.LastGameStorageItemKey);
    }
}