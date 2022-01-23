namespace Tofunaut.TofuECS_Rogue.ECS
{
    public unsafe struct Modifiable
    {
        public const int MaxAssignedModifiers = 16;

        public int NumAssignedModifiers;
        public fixed int AssignedModifiers[MaxAssignedModifiers];
    }
}