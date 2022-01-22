using System;
using Tofunaut.TofuECS;

namespace Tofunaut.TofuECS_Rogue.ECS
{
    public class FloorSystem : ISystem, ISystemEventListener<FloorTilesInputEvent>
    {
        public static event EventHandler<TilesUpdatedEventArgs> TilesUpdated;
        
        public void Initialize(Simulation s) { }

        public void Process(Simulation s) { }
        
        public unsafe void OnSystemEvent(Simulation s, in FloorTilesInputEvent eventData)
        {
            var gameState = s.GetSingletonComponentUnsafe<GameState>();
            var tilesBuffer = s.AnonymousBuffer<Tile>(gameState->TileBufferIndex);
            var tiles = eventData.Tiles;
            var indexes = new int[tiles.Length];
            for (var i = 0; i < tilesBuffer.Size && i < eventData.Tiles.Length; i++)
            {
                var tile = tilesBuffer.GetAtUnsafe(i);
                *tile = eventData.Tiles[i];
                indexes[i] = i;
            }
            
            TilesUpdated?.Invoke(this, new TilesUpdatedEventArgs(gameState->TileBufferIndex, indexes, tiles));
        }
    }

    public struct FloorTilesInputEvent
    {
        public Tile[] Tiles;
    }

    public class TilesUpdatedEventArgs : EventArgs
    {
        public readonly int TileBufferIndex;
        public readonly int[] Indexes;
        public readonly Tile[] Tiles;

        public TilesUpdatedEventArgs(int tileBufferIndex, int[] indexes, Tile[] tiles)
        {
            TileBufferIndex = tileBufferIndex;
            Indexes = indexes;
            Tiles = tiles;
        }
    }
}