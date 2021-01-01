using System.Collections.Generic;
using System.Linq;

namespace Blazor2048.Models
{
    public class Grid
    {
        public const int Size = 4;
        private readonly Cell[,] _cells;

        public Grid()
        {
            _cells = new Cell[Size, Size];

            for (var y = 0; y < Size; y++)
            for (var x = 0; x < Size; x++)
                _cells[y, x] = new();
        }

        public Cell this[int x, int y] => GetCell(x, y);

        private Cell GetCell(int x, int y) => _cells[y, x];

        private bool CanMerge()
        {
            for (var y = 0; y < Size; y++)
            {
                var left = 0;
                var top = 0;

                for (var x = 0; x < Size; x++)
                {
                    var hValue = GetCell(x, y).TileValue;
                    var vValue = GetCell(y, x).TileValue;

                    if (hValue == left || vValue == top)
                        return true;

                    left = hValue ?? 0;
                    top = vValue ?? 0;
                }
            }

            return false;
        }

        public bool HasEmptyCells() => EnumerateCells().Any(cell => !cell.TileValue.HasValue);

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

        public IEnumerable<Cell> EnumerateCells()
        {
            var enumerator = _cells.GetEnumerator();
            while (enumerator.MoveNext())
                if (enumerator.Current is Cell cell)
                    yield return cell;
        }
    }
}