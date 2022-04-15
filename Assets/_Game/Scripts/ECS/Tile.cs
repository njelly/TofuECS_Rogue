namespace Tofunaut.TofuECS_Rogue.ECS
{
    public enum Material
    {
        Void,
        Air,
        Stone,
        Water,
        Dirt,
    }
    
    public struct Tile
    {
        public int Height;
        public Material Material;
        public int Integrity;
        public bool IsBlocked;
    }
}