namespace Tofunaut.TofuECS_Rogue.ECS
{
    public struct WorldData
    {
        public int Width;
        public int FloorBufferIndex;
        
        public int IndexFromCoord(int x, int y) => Width * y + x;
        public (int x, int y) CoordFromIndex(int index) => (index % Width, index / Width);
    }
}