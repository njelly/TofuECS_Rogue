using Tofunaut.TofuECS;

namespace Tofunaut.TofuECS_Rogue.ECS
{
    public unsafe class UnitSystem : ISystem, ISystemEventListener<MoveUnitInput>, ISystemEventListener<CreateUnitInput>
    {
        public delegate void UnitDelegate(int entityId, in Unit unit);

        public static event UnitDelegate UnitMoved;
        public static event UnitDelegate UnitCreated;
        
        public void Initialize(Simulation s) { }

        public void Process(Simulation s) { }
        
        public void OnSystemEvent(Simulation s, in MoveUnitInput eventData)
        {
            var unitBuffer = s.Buffer<Unit>();
            if (!unitBuffer.GetUnsafe(eventData.EntityId, out var unit))
                return;

            var worldData = s.GetSingletonComponent<WorldData>();
            var tileBuffer = s.AnonymousBuffer<Tile>(worldData.TileBufferIndex);
            var newTileIndex = worldData.IndexFromCoord(eventData.NewX, eventData.NewY);
            
            // make sure the newTileIndex is valid
            if (newTileIndex >= 0 && newTileIndex < tileBuffer.Size)
            {
                var newTile = tileBuffer.GetAtUnsafe(newTileIndex);
            
                // if the new tile is not blocked, then we can move the unit to that tile
                if (!newTile->IsBlocked)
                {
                    // if this unit blocks tiles, unblock the tile we were currently occupying and block the new one
                    if (unit->DoesBlockTile)
                    {
                        var currentTile = tileBuffer.GetAtUnsafe(unit->TileIndex);
                        currentTile->IsBlocked = false;
                        newTile->IsBlocked = true;
                    }
                
                    unit->TileIndex = newTileIndex;
                }
            }
            
            unit->Facing = eventData.Facing;
            
            UnitMoved?.Invoke(eventData.EntityId, *unit);
        }

        public void OnSystemEvent(Simulation s, in CreateUnitInput eventData)
        {
            var entity = s.CreateEntity();
            s.Buffer<Unit>().Set(entity, eventData.Unit);

            if (eventData.Health != null)
                s.Buffer<Health>().Set(entity, eventData.Health.Value);
            
            UnitCreated?.Invoke(entity, eventData.Unit);
        }
    }
    
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