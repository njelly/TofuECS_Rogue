using System;
using Tofunaut.TofuECS;

namespace Tofunaut.TofuECS_Rogue.ECS
{
    public class TileSystem : ISystem, ISystemEventListener<SetTilesInput>
    {
        public delegate void TileUpdatedDelegate(int x, int y, in Tile tile);
        
        public static event TileUpdatedDelegate TileUpdated;

        public void Initialize(Simulation s) { }

        public void Process(Simulation s) { }

        public void OnSystemEvent(Simulation s, in SetTilesInput eventData)
        {
            var worldData = s.GetSingletonComponent<WorldData>();
            var tileBuffer = s.AnonymousBuffer<Tile>(worldData.TileBufferIndex);
            tileBuffer.SetState(eventData.Tiles);

            for (var i = 0; i < eventData.Tiles.Length; i++)
            {
                var (x, y) = worldData.CoordFromIndex(i);
                TileUpdated?.Invoke(x, y, eventData.Tiles[i]);
            }
        }
    }

    public struct SetTilesInput
    {
        public Tile[] Tiles;
    }
}