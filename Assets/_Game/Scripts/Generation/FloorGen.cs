using System;
using System.Threading.Tasks;
using Tofunaut.TofuECS_Rogue.ECS;
using Tofunaut.TofuECS.Utilities;

namespace Tofunaut.TofuECS_Rogue.Generation
{
    public class FloorGenParams
    {
        public int Seed;
        public int MinRoomSize;
        public int MaxRoomSize;
    }
    
    public static class FloorGen
    {
        public static async void RequestFloor(FloorGenParams floorGenParams, Action<Tile[]> onComplete)
        {
            var tiles = await Task.Run(() => GenerateFloor(floorGenParams));
            onComplete?.Invoke(tiles);
        }
        
        private static Tile[] GenerateFloor(FloorGenParams floorGenParams)
        {
            var r = new XorShiftRandom(Convert.ToUInt64(floorGenParams.Seed));
            var toReturn = new Tile[Floor.FloorSize * Floor.FloorSize];
            for (var i = 0; i < toReturn.Length; i++)
            {
                if (i is < Floor.FloorSize or > Floor.FloorSize * Floor.FloorSize - Floor.FloorSize || i % Floor.FloorSize == 0 || i % Floor.FloorSize == Floor.FloorSize - 1)
                {
                    toReturn[i].Type = TileType.Bedrock;
                }
                else
                {
                    toReturn[i].Type = TileType.Stone;
                }

                toReturn[i].Height = Tile.WallHeight;
            }

            var roomWidth = (int) (r.NextDouble() * floorGenParams.MaxRoomSize) - floorGenParams.MinRoomSize +
                            floorGenParams.MinRoomSize;
            var roomHeight = (int) (r.NextDouble() * floorGenParams.MaxRoomSize) - floorGenParams.MinRoomSize +
                             floorGenParams.MinRoomSize;

            var minX = 1 + (int) (r.NextDouble() * Floor.FloorSize - roomWidth - 1);
            var minY = 1 + (int) (r.NextDouble() * Floor.FloorSize - roomHeight - 1);

            for (var x = minX; x < minX + roomWidth; x++)
            {
                for (var y = minY; y < minY + roomHeight; y++)
                {
                    var i = x + y * Floor.FloorSize;
                    toReturn[i].Height = Tile.FloorHeight;
                }
            }
            
            return toReturn;
        }
    }
}