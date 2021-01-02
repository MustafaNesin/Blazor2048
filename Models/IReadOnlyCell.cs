namespace Blazor2048.Models
{
    public interface IReadOnlyCell
    {
        CellAnimation Animation { get; }
        int? TileValue { get; }
        int? OldTileValue { get; }
    }
}