using System;
using System.Collections.Generic;
using System.Linq;

namespace Blazor2048.Models
{
    public class Game
    {
        private const int WinningTileValue = 2048;
        private const int StartTiles = 2;
        private readonly Random _random;
        private int _previousScore;

        public Game()
        {
            _random = new();
            Grid = new();

            for (var i = 0; i < StartTiles; i++)
                AddRandomTile();
        }

        internal Game(int score, int?[] tileValues)
        {
            _random = new();
            Grid = new();

            Score = score;
            for (var y = 0; y < Grid.Size; y++)
            for (var x = 0; x < Grid.Size; x++)
                Grid[x, y].TileValue = tileValues[y * Grid.Size + x];
        }

        public bool Over { get; private set; }
        public bool Won { get; private set; }
        public int Score { get; private set; }
        public Grid Grid { get; }

        public bool CanUndo { get; private set; }

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

        public void Undo()
        {
            if (!CanUndo)
                return;

            foreach (var cell in Grid.EnumerateCells())
            {
                var tileValue = cell.TileValue;
                cell.TileValue = cell.OldTileValue;
                cell.OldTileValue = tileValue;

                cell.Animation = cell.Animation switch
                {
                    CellAnimation.ZoomIn => CellAnimation.ZoomOut,
                    CellAnimation.ZoomOut => CellAnimation.None,
                    _ => cell.Animation
                };
            }

            Score = _previousScore;
            Over = false;
            CanUndo = false;
        }

        public bool Move(Direction direction)
        {
            if (Over)
                return false;

            var moved = false;
            var traversals = Grid.EnumerateTraversals(direction);
            _previousScore = Score;

            foreach (var traversal in traversals)
                moved |= MoveTraversal(traversal);

            if (!moved)
                return false;

            AddRandomTile();
            CanUndo = true;

            if (!Grid.CanMove())
                Over = true;

            return true;
        }

        private bool MoveTraversal(IReadOnlyList<Cell> traversal)
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
                        Won = true;
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