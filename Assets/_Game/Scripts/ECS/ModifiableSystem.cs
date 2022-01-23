using System;
using Tofunaut.TofuECS;
using UnsafeCollections.Collections.Unsafe;

namespace Tofunaut.TofuECS_Rogue.ECS
{
    public class ModifiableSystem : ISystem
    {
        public void Initialize(Simulation s) { }

        public unsafe void Process(Simulation s)
        {
            var playerInput = s.GetSingletonComponent<PlayerInput>();
            var modifierBuffer = s.Buffer<Modifier>();
            var modifiableBuffer = s.Buffer<Modifiable>();
            var query = s.Query<Modifier>();
            var toRemove = stackalloc int[query.Entities.Count];
            var numToRemove = 0;
            foreach(var entity in query.Entities)
            {
                modifierBuffer.GetUnsafe(entity, out var modifier);
                if (!modifiableBuffer.GetUnsafe(modifier->ModifiableEntity, out var modifiable))
                {
                    // modifiers whose modifiables don't exist need to be removed
                    toRemove[numToRemove++] = entity;
                }
                else
                {
                    // allow negative time left values to exist until they are explicitly removed
                    if(modifier->TimeLeft < 0)
                        continue;

                    modifier->TimeLeft -= playerInput.DeltaTime;
                    if (modifier->TimeLeft > 0)
                        continue;

                    modifier->ModifiableEntity = Simulation.InvalidEntityId;
                    RemoveModifierFromModifiable(s, modifiable, entity);
                }
            }

            for (var i = 0; i < numToRemove; i++)
                modifierBuffer.Remove(toRemove[i]);
        }

        public static unsafe int AssignModifierToModifiable(Simulation s, Modifiable* modifiable, in Modifier modifier, StackBehavior stackBehavior)
        {
            if (modifiable->NumAssignedModifiers >= Modifiable.MaxAssignedModifiers)
            {
                s.Log.Error($"Too many modifiers assigned to modifiable.");
                return Simulation.InvalidEntityId;
            }

            var modifierBuffer = s.Buffer<Modifier>();
            
            if (stackBehavior is StackBehavior.DoesNotStack or StackBehavior.RefreshDuration)
            {
                var stackedIndex = -1;
                
                for (var i = 0; i < modifiable->NumAssignedModifiers; i++)
                {
                    if (!modifierBuffer.Get(modifiable->ModifierEntities[i], out var assignedModifier) ||
                        assignedModifier.ModifierClass != modifier.ModifierClass)
                        continue;
                    
                    stackedIndex = i;
                    break;
                }

                if (stackedIndex != -1)
                {
                    switch (stackBehavior)
                    {
                        // if the behavior is "DoesNotStack" and a modifier with the same class has already been assigned, return immediately
                        case StackBehavior.DoesNotStack:
                            return Simulation.InvalidEntityId;
                        // if the behavior is RefreshDuration, then just set the TimeLeft value and return
                        case StackBehavior.RefreshDuration when modifierBuffer.GetUnsafe(modifiable->ModifierEntities[stackedIndex], out var originalModifier):
                            originalModifier->TimeLeft = Math.Max(originalModifier->TimeLeft, modifier.TimeLeft);
                            return Simulation.InvalidEntityId;
                    }
                }
            }

            var modifierEntity = s.CreateEntity();
            modifierBuffer.Set(modifierEntity, modifier);
            modifiable->ModifierEntities[modifiable->NumAssignedModifiers++] = modifierEntity;
            return modifierEntity;
        }

        public static unsafe void RemoveModifierFromModifiable(Simulation s, Modifiable* modifiable, int modifierEntity)
        {
            var index = 0;
            for (; index < modifiable->NumAssignedModifiers; index++)
            {
                if (modifiable->ModifierEntities[index] == modifierEntity)
                    break;
            }

            if (index >= modifiable->NumAssignedModifiers)
                return;

            // Setting the modifiable entity to invalid will cause the modifier to be removed from its own entity the next
            // time ModifiableSystem.Process() is called.
            if (s.Buffer<Modifier>().GetUnsafe(modifierEntity, out var modifier))
                modifier->ModifiableEntity = Simulation.InvalidEntityId;

            // this makes sure modifier entities are always ordered by when they were assigned
            for (var i = index; i < modifiable->NumAssignedModifiers; i++)
                modifiable->ModifierEntities[i] = modifiable->ModifierEntities[i + 1];
            
            modifiable->NumAssignedModifiers--;
        }

        public static unsafe bool TryGetValueForModifierClass(Simulation s, in Modifiable modifiable, ModifierClass modifierClass, StackBehavior stackBehavior, out float totalValue)
        {
            var mainValue = 0f;
            var stackValue = 0f;
            var modifierBuffer = s.Buffer<Modifier>();
            var hasTalliedFirstOfClass = false;
            for (var i = 0; i < modifiable.NumAssignedModifiers; i++)
            {
                if (!modifierBuffer.Get(modifiable.ModifierEntities[i], out var modifier))
                    continue;

                if (modifier.ModifierClass != modifierClass)
                    continue;

                if (!hasTalliedFirstOfClass)
                {
                    mainValue = modifier.Value;
                    hasTalliedFirstOfClass = true;
                }
                else
                {
                    stackValue += modifier.StackedValue;
                }
            }

            if (!hasTalliedFirstOfClass)
            {
                totalValue = 0f;
                return false;
            }

            switch (stackBehavior)
            {
                case StackBehavior.RefreshDuration:
                case StackBehavior.DoesNotStack:
                    totalValue = mainValue;
                    break;
                case StackBehavior.Add:
                    totalValue = mainValue + stackValue;
                    break;
                case StackBehavior.Multiply:
                    totalValue = mainValue * stackValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stackBehavior), stackBehavior, null);
            }

            return true;
        }
    }
}