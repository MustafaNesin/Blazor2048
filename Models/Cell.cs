namespace Blazor2048.Models
{
    public class Cell : ICell, IReadOnlyCell
    {
        public CellAnimation Animation { get; set; }
        public int? TileValue { get; set; }
        public int? OldTileValue { get; set; }
    }
}