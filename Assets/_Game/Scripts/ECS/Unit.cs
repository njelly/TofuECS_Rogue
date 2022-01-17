namespace Tofunaut.TofuECS_Rogue.ECS
{
    public enum ViewId
    {
        None = 0,
        Test = 1,
    }
    
    public struct Unit
    {
        // movement
        public bool IsMoving => TimeToDestination > 0;
        public int PrevX;
        public int PrevY;
        public int X;
        public int Y;
        public float TimeToDestination;
        public float MoveSpeed;
        
        // view
        public ViewId CurrentViewId;
        
        // input
        public float InputDirX;
        public float InputDirY;
    }
}