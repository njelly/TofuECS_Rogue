using Tofunaut.TofuECS;

namespace Tofunaut.TofuECS_Rogue.ECS
{
    public unsafe class UnitSystem : ISystem, ISystemEventListener<MoveUnitInput>
    {
        public delegate void UnitMovedDelegate(int entityId, in Unit unit);

        public static event UnitMovedDelegate UnitMoved;
        
        public void Initialize(Simulation s) { }

        public void Process(Simulation s) { }
        
        public void OnSystemEvent(Simulation s, in MoveUnitInput eventData)
        {
            var unitBuffer = s.Buffer<Unit>();
            if (!unitBuffer.GetUnsafe(eventData.EntityId, out var unit))
                return;
            
            var tileBuffer = s.AnonymousBuffer<Tile>(unit->TileBufferIndex);
            var newTileIndex = s.GetSingletonComponent<WorldData>().IndexFromCoord(eventData.NewX, eventData.NewY);
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
            unit->Facing = eventData.Facing;
            
            UnitMoved?.Invoke(eventData.EntityId, *unit);
        }
    }
}