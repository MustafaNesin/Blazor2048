using System;
using System.Collections.Generic;
using System.Linq;

namespace Blazor2048.Models
{
    public class Game : IGame
    {
        private readonly Random _random;
        private int _oldScore;

        public Game(int gridSize, int startTiles, int winningTileValue)
        {
            _random = new();
            WinningTileValue = winningTileValue;
            Grid = new Grid(gridSize);

            for (var i = 0; i < startTiles; i++)
                AddRandomTile();
        }

        public Game(int winningTileValue, int score, IList<int?> tileValues)
        {
            _random = new();
            Score = score;
            WinningTileValue = winningTileValue;
            IsWon = tileValues.Any(value => value >= WinningTileValue);
            Grid = new Grid((int)Math.Sqrt(tileValues.Count));

            for (var y = 0; y < Grid.Size; y++)
            for (var x = 0; x < Grid.Size; x++)
                Grid[x, y].TileValue = tileValues[y * Grid.Size + x];

            IsOver = !Grid.CanMove();
        }

        public int Score { get; private set; }
        public bool IsOver { get; private set; }
        public bool IsWon { get; private set; }
        public bool CanUndo { get; private set; }
        public IGrid Grid { get; }
        public int WinningTileValue { get; }

        public bool Move(Direction direction)
        {
            var moved = false;
            var traversals = Grid.EnumerateTraversals(direction);
            _oldScore = Score;

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var traversal in traversals)
                moved |= MoveTraversal(traversal);

            if (!moved)
                return false;

            AddRandomTile();

            if (!Grid.CanMove())
                IsOver = true;

            CanUndo = true;
            return true;
        }

        public bool Undo()
        {
            if (!CanUndo)
                return false;

            foreach (var cell in Grid.EnumerateCells())
            {
                var tileValue = cell.TileValue;
                cell.TileValue = cell.OldTileValue;
                cell.OldTileValue = tileValue;

                // ReSharper disable once ConvertIfStatementToSwitchExpression
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (cell.Animation == CellAnimation.ZoomIn)
                    cell.Animation = CellAnimation.ZoomOut;
                else if (cell.Animation == CellAnimation.ZoomOut)
                    cell.Animation = CellAnimation.None;
            }

            Score = _oldScore;
            IsOver = false;
            IsWon = Grid.EnumerateCells().Any(cell => cell.TileValue.HasValue && cell.TileValue >= WinningTileValue);
            CanUndo = false;
            return true;
        }

        private void AddRandomTile()
        {
            var emptyCells = Grid.EnumerateCells().Where(cell => !cell.TileValue.HasValue).ToArray();

            if (emptyCells.Length == 0)
                return;

            var emptyCell = emptyCells[_random.Next(0, emptyCells.Length)];
            var tileValue = _random.NextDouble() < 0.9 ? 2 : 4;

            emptyCell.TileValue = tileValue;
            emptyCell.Animation = CellAnimation.ZoomIn;
        }

        private bool MoveTraversal(IReadOnlyList<ICell> traversal)
        {
            var finalValues = new int?[traversal.Count];
            var tail = -1;
            var canMerge = false;
            var moved = false;
            var mergedIndices = new List<int>();

            foreach (var current in traversal)
            {
                if (!current.TileValue.HasValue)
                    continue;

                canMerge &= tail > -1 && finalValues[tail] == current.TileValue;

                if (canMerge)
                {
                    Score += (int)(finalValues[tail] *= 2)!;
                    mergedIndices.Add(tail);

                    if (finalValues[tail] == WinningTileValue)
                        IsWon = true;
                }
                else
                    finalValues[++tail] = current.TileValue;

                canMerge ^= true;
            }

            for (var i = 0; i < traversal.Count; i++)
            {
                moved |= traversal[i].TileValue != finalValues[i];
                traversal[i].OldTileValue = traversal[i].TileValue;
                traversal[i].TileValue = finalValues[i];
                traversal[i].Animation = mergedIndices.Contains(i) ? CellAnimation.BounceIn : CellAnimation.None;
            }

            return moved;
        }
    }
}