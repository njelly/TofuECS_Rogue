namespace Tofunaut.TofuECS_Rogue.ECS
{
    public enum Material
    {
        Void,
        Bedrock,
        Air,
        Stone,
        Water,
        Dirt,
    }
    
    public struct Tile
    {
        public const int FallThroughHeight = -2;
        public const int HoleHeight = -1;
        public const int FloorHeight = 0;
        public const int UnitHeight = 1;
        public const int FlyingHeight = 2;
        public const int CeilingHeight = 3;
        
        public int Height;
        public Material Material;
        public int Integrity;
        public bool IsBlocked;
    }
}