namespace Tofunaut.TofuECS_Rogue.ECS
{
    public unsafe struct Modifiable
    {
        public const int MaxModifiables = Unit.MaxUnits;
        public const int MaxAssignedModifiers = 16;
        
        public int NumAssignedModifiers;
        public fixed int ModifierEntities[MaxAssignedModifiers];
    }
}