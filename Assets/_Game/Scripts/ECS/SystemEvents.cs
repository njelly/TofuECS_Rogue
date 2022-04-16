namespace Tofunaut.TofuECS_Rogue.ECS
{
    public struct MoveUnitInput
    {
        public int EntityId;
        public int NewX;
        public int NewY;
        public Facing Facing;
    }

    public struct CreateUnitInput
    {
        public Unit Unit;
        public Health? Health;
    }
}