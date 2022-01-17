namespace Tofunaut.TofuECS_Rogue.ECS
{
    public struct Unit
    {
        public bool IsMoving => TimeToDestination > 0;
        
        public int PrevX;
        public int PrevY;
        public int X;
        public int Y;
        public float TimeToDestination;
    }
}