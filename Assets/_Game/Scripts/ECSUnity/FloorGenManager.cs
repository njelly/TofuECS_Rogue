using System;
using Tofunaut.TofuECS_Rogue.ECS;
using Tofunaut.TofuECS_Rogue.Generation;

namespace Tofunaut.TofuECS_Rogue.ECSUnity
{
    public class FloorGenManager
    {
        public async void RequestFloor(Action<Tile[]> onComplete)
        {
            var tiles = await FloorGen.GenerateFloorAsync(new FloorGenParams
            {
                MaxRoomSize = 20,
                MinRoomSize = 10,
                Seed = 1234,
            });
            onComplete?.Invoke(tiles);
        }
    }
}