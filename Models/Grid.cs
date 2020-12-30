using System.Collections.Generic;
using System.Linq;

namespace Blazor2048.Models
{
    public class Grid
    {
        private readonly Cell[,] _cells;

        public Grid(int size)
        {
            Size = size;
            _cells = new Cell[Size, Size];

            for (var y = 0; y < Size; y++)
            for (var x = 0; x < Size; x++)
                _cells[y, x] = new(x, y);
        }

        public int Size { get; }

        public int? this[int x, int y]
        {
            get => GetTileValue(x, y);
            set => SetTileValue(x, y, value);
        }

        private Cell GetCell(int x, int y) => _cells[y, x];
        public int? GetTileValue(int x, int y) => GetCell(x, y).TileValue;
        public void SetTileValue(int x, int y, int? tileValue) => GetCell(x, y).TileValue = tileValue;

        public bool CanMerge()
        {
            for (var y = 0; y < Size; y++)
            {
                var left = 0;
                var top = 0;

                for (var x = 0; x < Size; x++)
                {
                    var hValue = GetTileValue(x, y);
                    var vValue = GetTileValue(y, x);

                    if (hValue == left || vValue == top)
                        return true;

                    left = hValue ?? 0;
                    top = vValue ?? 0;
                }
            }

            return false;
        }

        public bool HasEmptyCells() => EnumerateEmptyCells().Any();

        public bool CanMove() => CanMerge() || HasEmptyCells();

        public IEnumerable<Cell[]> EnumerateTraversals(Direction direction)
        {
            var isHorizontal = direction == Direction.Left || direction == Direction.Right;
            var isPositive = direction == Direction.Right || direction == Direction.Down;

            for (var y = 0; y < Size; y++)
            {
                var traversal = new Cell[Size];

                for (var x = 0; x < Size; x++)
                    traversal[isPositive ? Size - 1 - x : x] = isHorizontal ? GetCell(x, y) : GetCell(y, x);

                yield return traversal;
            }
        }

        public IEnumerable<Cell> EnumerateEmptyCells()
        {
            var enumerator = _cells.GetEnumerator();
            while (enumerator.MoveNext())
                if (enumerator.Current is Cell { TileValue: null } cell)
                    yield return cell;
        }
    }
}