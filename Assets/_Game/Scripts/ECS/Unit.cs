using System.Numerics;

namespace Tofunaut.TofuECS_Rogue.ECS
{

    
    public enum ViewId
    {
        None = 0,
        Test = 1,
        Player = 2,
    }
    
    public struct Unit
    {
        // movement
        public bool IsMoving => CurrentPos != TargetPos;
        public Vector2 TargetPos;
        public Vector2 CurrentPos;
        public CardinalDirection4 Facing;
        public float MoveSpeed;
        
        // view
        public ViewId CurrentViewId;
        
        // input
        public UnitInput Input;
    }
    
    public struct UnitInput
    {
        public const int FaceThreshold = 1;
        public const int MoveThreshold = 2;
        public CardinalDirection4 Dir;
        public int DirMagnitude;
    }
}