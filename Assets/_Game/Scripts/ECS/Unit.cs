namespace Tofunaut.TofuECS_Rogue.ECS
{
    public struct Unit
    {
        public int TileIndex;
        public int TileBufferIndex; // aka, the current floor
        public Facing Facing;
        public int ViewId;
        public bool DoesBlockTile; // does this unit block other units from occupying a tile?
    }
}