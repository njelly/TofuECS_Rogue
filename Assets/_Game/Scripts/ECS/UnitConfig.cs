using System.Numerics;

namespace Tofunaut.TofuECS_Rogue.ECS
{
    public struct UnitConfig
    {
        public Vector2 Position;
        public ViewId ViewId;
        public float MoveSpeed;
        public CardinalDirection4 InitFacing;
    }
}