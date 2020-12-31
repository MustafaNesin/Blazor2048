namespace Blazor2048.Models
{
    public class Cell
    {
        public int? TileValue { get; set; }
        public int? PreviousTileValue { get; set; }

        public override string ToString() => TileValue.HasValue ? TileValue.Value.ToString() : string.Empty;
    }
}