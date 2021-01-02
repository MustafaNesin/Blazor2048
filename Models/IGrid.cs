using System.Collections.Generic;

namespace Blazor2048.Models
{
    public interface IGrid
    {
        ICell this[int x, int y] { get; }
        int Size { get; }
        bool CanMove();
        IEnumerable<ICell[]> EnumerateTraversals(Direction direction);
        IEnumerable<ICell> EnumerateCells();
    }
}