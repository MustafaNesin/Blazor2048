namespace Blazor2048.Models
{
    public interface ICell
    {
        CellAnimation Animation { get; set; }
        int? TileValue { get; set; }
        int? OldTileValue { get; set; }
    }
}