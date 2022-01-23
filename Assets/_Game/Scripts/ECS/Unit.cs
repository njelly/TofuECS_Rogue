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
        public const int MaxUnits = 16;
        
        // movement
        public bool IsMoving => CurrentPos != TargetPos;
        public Vector2 TargetPos;
        public Vector2 CurrentPos;
        public CardinalDirection4 Facing;
        public float MoveSpeed;
        public int SprintModifierEntity;
        
        // view
        public ViewId CurrentViewId;
        
        // input
        public UnitInput Input;
    }
    
    public struct UnitInput
    {
        public const int FaceThreshold = 1;
        public const int MoveThreshold = 2;
        public const int SprintThreshold = 3;
        public CardinalDirection4 Dir;
        public int DirMagnitude;
    }
}