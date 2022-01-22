namespace Tofunaut.TofuECS_Rogue.ECS
{
    public enum TileType
    {
        Void = 0,
        Bedrock = 1,
        Stone = 2,
        LadderDown = 3,
        LadderUp = 4,
    }

    public struct Tile
    {
        public const int FloorHeight = 0;
        public const int UnitHeight = 1;
        public const int WallHeight = 2;
        
        public TileType Type;
        public int Height;
    }
}