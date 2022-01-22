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

    public class FloorGenResult
    {
        public Tile[] Tiles;
        public int PlayerSpawnX;
        public int PlayerSpawnY;
    }
    
    public static class FloorGen
    {
        public static async void RequestFloor(FloorGenParams floorGenParams, Action<FloorGenResult> onComplete)
        {
            var result = await Task.Run(() => GenerateFloor(floorGenParams));
            onComplete?.Invoke(result);
        }

        public static async Task<FloorGenResult> RequestFloorAsync(FloorGenParams floorGenParams)
        {
            return await Task.Run(() => GenerateFloor(floorGenParams));
        }
        
        private static FloorGenResult GenerateFloor(FloorGenParams floorGenParams)
        {
            var r = new XorShiftRandom(Convert.ToUInt64(floorGenParams.Seed));
            var tiles = new Tile[Floor.FloorSize * Floor.FloorSize];
            for (var i = 0; i < tiles.Length; i++)
            {
                if (i is < Floor.FloorSize or > Floor.FloorSize * Floor.FloorSize - Floor.FloorSize || i % Floor.FloorSize == 0 || i % Floor.FloorSize == Floor.FloorSize - 1)
                {
                    tiles[i].Type = TileType.Bedrock;
                }
                else
                {
                    tiles[i].Type = TileType.Stone;
                }

                tiles[i].Height = Tile.WallHeight;
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
                    tiles[i].Height = Tile.FloorHeight;
                }
            }

            var ladderUpX = minX + 1 + (int) (r.NextDouble() * roomWidth - 1);
            var ladderUpY = minY + 1 + (int) (r.NextDouble() * roomHeight - 1);

            tiles[ladderUpX + ladderUpY * Floor.FloorSize].Height = Tile.WallHeight;
            tiles[ladderUpX + ladderUpY * Floor.FloorSize].Type = TileType.LadderUp;
            
            return new FloorGenResult
            {
                PlayerSpawnX = ladderUpX,
                PlayerSpawnY = ladderUpY,
                Tiles = tiles,
            };
        }
    }
}