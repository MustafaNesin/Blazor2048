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

        public Game()
        {
            _random = new();
            Grid = new();

            for (var i = 0; i < StartTiles; i++)
                AddRandomTile();
        }

        public bool Over { get; private set; }
        public bool Won { get; private set; }
        public int Score { get; private set; }
        public Grid Grid { get; }

        private void AddRandomTile()
        {
            var emptyCells = Grid.EnumerateEmptyCells().ToArray();

            if (emptyCells.Length == 0)
                return;

            var emptyCell = emptyCells[_random.Next(0, emptyCells.Length)];
            var tileValue = _random.NextDouble() < 0.9 ? 2 : 4;

            emptyCell.TileValue = tileValue;
        }

        public bool Move(Direction direction)
        {
            if (Over)
                return false;

            var moved = false;

            var traversals = Grid.EnumerateTraversals(direction);
            foreach (var traversal in traversals)
                moved |= MoveTraversal(traversal);

            if (!moved)
                return false;

            AddRandomTile();

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

            foreach (var current in traversal)
            {
                if (current.TileValue is null)
                    continue;

                canMerge &= tail > -1 && finalValues[tail] == current.TileValue;

                if (canMerge)
                {
                    Score += (int)(finalValues[tail] *= 2)!;

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
                traversal[i].TileValue = finalValues[i];
            }

            return moved;
        }
    }
}