namespace HopeOfTheAncients.Tiled
{
    public interface IMapDataProvider
    {
        IChunkDataProvider ChunkDataProvider { get; }
        ITileDataProvider TileDataProvider { get; }
    }
}