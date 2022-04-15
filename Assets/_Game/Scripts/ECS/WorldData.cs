namespace Tofunaut.TofuECS_Rogue.ECS
{
    public unsafe struct WorldData
    {
        public const int MaxFloorsInMemory = 8;
        
        public int Width;
        public fixed int FloorBufferIndexes[MaxFloorsInMemory];

    }

    public static class WorldDataExtensions
    {
        public static int IndexFromCoord(this WorldData worldData, int x, int y) => worldData.Width * y + x;
        public static (int x, int y) CoordFromIndex(this WorldData worldData, int index) => (index % worldData.Width, index / worldData.Width);
    }
}