using System.Collections.Generic;
using System.Linq;

namespace Blazor2048.Models
{
    public class Grid : IGrid
    {
        private readonly Cell[,] _cells;

        public Grid(int size)
        {
            Size = size;
            _cells = new Cell[Size, Size];

            for (var y = 0; y < Size; y++)
            for (var x = 0; x < Size; x++)
                _cells[y, x] = new();
        }

        public int Size { get; }

        public ICell this[int x, int y] => _cells[y, x];

        public bool CanMove() => CanMerge() || HasEmptyCells();

        public IEnumerable<ICell[]> EnumerateTraversals(Direction direction)
        {
            var isHorizontal = direction == Direction.Left || direction == Direction.Right;
            var isPositive = direction == Direction.Right || direction == Direction.Down;

            for (var y = 0; y < Size; y++)
            {
                var traversal = new ICell[Size];

                for (var x = 0; x < Size; x++)
                    traversal[isPositive ? Size - 1 - x : x] = isHorizontal ? _cells[y, x] : _cells[x, y];

                yield return traversal;
            }
        }

        public IEnumerable<ICell> EnumerateCells()
        {
            var enumerator = _cells.GetEnumerator();
            while (enumerator.MoveNext())
                if (enumerator.Current is Cell cell)
                    yield return cell;
        }

        private bool CanMerge()
        {
            for (var y = 0; y < Size; y++)
            {
                var left = 0;
                var top = 0;

                for (var x = 0; x < Size; x++)
                {
                    var hValue = _cells[y, x].TileValue;
                    var vValue = _cells[x, y].TileValue;

                    if (hValue == left || vValue == top)
                        return true;

                    left = hValue ?? 0;
                    top = vValue ?? 0;
                }
            }

            return false;
        }

        private bool HasEmptyCells() => EnumerateCells().Any(cell => !cell.TileValue.HasValue);
    }
}