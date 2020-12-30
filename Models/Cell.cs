namespace Blazor2048.Models
{
    public class Cell
    {
        public Cell(int x, int y) => (X, Y) = (x, y);
        public int X { get; }
        public int Y { get; }
        public int? TileValue { get; set; }
    }
}