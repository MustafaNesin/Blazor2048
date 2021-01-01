namespace Blazor2048.Models
{
    public class Cell
    {
        public CellAnimation Animation { get; set; }
        public int? TileValue { get; set; }
        public int? OldTileValue { get; set; }

        public override string ToString() => TileValue.HasValue ? TileValue.Value.ToString() : string.Empty;
    }
}