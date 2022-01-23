namespace Tofunaut.TofuECS_Rogue.ECS
{
    public enum ModifierClass
    {
        MoveSpeed,
    }

    public enum StackBehavior
    {
        DoesNotStack,
        Add,
        Multiply,
        RefreshDuration,
    }
    
    public struct Modifier
    {
        public ModifierClass ModifierClass;
        public float Value;
        public float StackedValue;
        public float TimeLeft;
        public int ModifiableEntity;
    }
}