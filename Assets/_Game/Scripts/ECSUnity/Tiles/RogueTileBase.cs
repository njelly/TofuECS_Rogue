using Tofunaut.TofuECS;
using Tofunaut.TofuECS_Rogue.ECS;
using UnityEngine;
using UnityEngine.Tilemaps;
using Tile = UnityEngine.Tilemaps.Tile;

namespace Tofunaut.TofuECS_Rogue.ECSUnity.Tiles
{
    public abstract class RogueTileBase : TileBase
    {
        public AnonymousBuffer<ECS.Tile> TileBuffer;
        [HideInInspector] public int ECSTileIndex;
        public Color Tint;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = PickSprite();
            tileData.color = Tint;
            tileData.colliderType = Tile.ColliderType.None;
        }

        protected abstract Sprite PickSprite();
        public ECS.Tile GetECSTile() => TileBuffer.GetAt(ECSTileIndex);

        /// <summary>
        /// Check if the tile is on an edge (i.e., it's north, south, east, or west neighbors are not identical).
        /// </summary>
        public bool IsOnEdge()
        {
            var tile = GetECSTile();
            if(TryGetNorthNeighbor(out var neighbor) && (neighbor.Type != tile.Type || neighbor.Height != tile.Height))
                return true;
            if(TryGetSouthNeighbor(out neighbor) && (neighbor.Type != tile.Type || neighbor.Height != tile.Height))
                return true;
            if(TryGetEastNeighbor(out neighbor) && (neighbor.Type != tile.Type || neighbor.Height != tile.Height))
                return true;
            return TryGetWestNeighbor(out neighbor) && (neighbor.Type != tile.Type || neighbor.Height != tile.Height);
        }

        public bool TryGetNorthNeighbor(out ECS.Tile northNeighbor)
        {
            if (ECSTileIndex > TileBuffer.Size - Floor.MaxFloorSize)
            {
                northNeighbor = default;
                return false;
            }

            northNeighbor = TileBuffer.GetAt(ECSTileIndex + Floor.MaxFloorSize);
            return true;
        }
        
        public bool TryGetEastNeighbor(out ECS.Tile eastNeighbor)
        {
            if (ECSTileIndex % Floor.MaxFloorSize == Floor.MaxFloorSize - 1)
            {
                eastNeighbor = default;
                return false;
            }

            eastNeighbor = TileBuffer.GetAt(ECSTileIndex + 1);
            return true;
        }
        
        public bool TryGetSouthNeighbor(out ECS.Tile southNeighbor)
        {
            if (ECSTileIndex < Floor.MaxFloorSize)
            {
                southNeighbor = default;
                return false;
            }

            southNeighbor = TileBuffer.GetAt(ECSTileIndex - Floor.MaxFloorSize);
            return true;
        }
        
        public bool TryGetWestNeighbor(out ECS.Tile westNeighbor)
        {
            if (ECSTileIndex % Floor.MaxFloorSize == 0)
            {
                westNeighbor = default;
                return false;
            }

            westNeighbor = TileBuffer.GetAt(ECSTileIndex - 1);
            return true;
        }
    }
}