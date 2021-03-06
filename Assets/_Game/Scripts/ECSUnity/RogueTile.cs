using System;
using Tofunaut.TofuECS;
using Tofunaut.TofuECS_Rogue.ECS;
using UnityEngine;
using UnityEngine.Tilemaps;
using Tile = UnityEngine.Tilemaps.Tile;

namespace Tofunaut.TofuECS_Rogue.ECSUnity
{
    [CreateAssetMenu(menuName = "TofuECS_Rogue/RogueTile")]
    public sealed class RogueTile : TileBase
    {
        public AnonymousBuffer<ECS.Tile> Buffer;

        [Header("Colors")] 
        [SerializeField] private Color _darkStoneColor;
        [SerializeField] private Color _mediumStoneColor;
        [SerializeField] private Color _darkWoodColor;
        [SerializeField] private Color _lightWoodColor;
        [SerializeField] private Color _darkPlayerColor;
        [SerializeField] private Color _lightPlayerColor;
        [SerializeField] private Color _mediumHighlightColor;
        [SerializeField] private Color _brightHighlightColor;
        
        [Header("Sprites")]
        [SerializeField] private Sprite _solidSprite;
        [SerializeField] private Sprite _blockSeparated;
        [SerializeField] private Sprite _ladderUp;
        [SerializeField] private Sprite _ladderDown;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            var i = position.x + position.y * Floor.FloorSize;
            var ecsTile = Buffer.GetAt(i);

            tileData.color = PickColor(i, ecsTile);
            tileData.sprite = PickSprite(i, ecsTile);
            tileData.colliderType = Tile.ColliderType.None;
        }

        private Color PickColor(int index, in ECS.Tile ecsTile)
        {
            return ecsTile.Type switch
            {
                TileType.Void => Color.clear,
                TileType.Bedrock => _darkStoneColor,
                TileType.Stone => _darkStoneColor,
                TileType.LadderDown => _brightHighlightColor,
                TileType.LadderUp => _brightHighlightColor,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private Sprite PickSprite(int index, in ECS.Tile ecsTile)
        {
            return ecsTile.Type switch
            {
                TileType.Void => null,
                TileType.Bedrock => _solidSprite,
                TileType.Stone => PickStoneSprite(index, ecsTile),
                TileType.LadderDown => _ladderDown,
                TileType.LadderUp => _ladderUp,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private Sprite PickStoneSprite(int index, in ECS.Tile ecsTile)
        {
            if (ecsTile.Height >= ECS.Tile.WallHeight)
                return IsOnEdge(Buffer, index, ecsTile) ? _blockSeparated : _solidSprite;

            return null;
        }

        private static bool IsOnEdge(AnonymousBuffer<ECS.Tile> buffer, int index, in ECS.Tile ecsTile)
        {
            if (TryGetNorthNeighbor(buffer, index, out var neighbor) &&
                (neighbor.Type != ecsTile.Type || neighbor.Height != ecsTile.Height))
                return true;

            if (TryGetNorthEastNeighbor(buffer, index, out neighbor) &&
                (neighbor.Type != ecsTile.Type || neighbor.Height != ecsTile.Height))
                return true;

            if (TryGetEastNeighbor(buffer, index, out neighbor) &&
                (neighbor.Type != ecsTile.Type || neighbor.Height != ecsTile.Height))
                return true;

            if (TryGetSouthEastNeighbor(buffer, index, out neighbor) &&
                (neighbor.Type != ecsTile.Type || neighbor.Height != ecsTile.Height))
                return true;

            if (TryGetSouthNeighbor(buffer, index, out neighbor) &&
                (neighbor.Type != ecsTile.Type || neighbor.Height != ecsTile.Height))
                return true;

            if (TryGetWestNeighbor(buffer, index, out neighbor) &&
                (neighbor.Type != ecsTile.Type || neighbor.Height != ecsTile.Height))
                return true;
            
            return TryGetNorthWestNeighbor(buffer, index, out neighbor) &&
                   (neighbor.Type != ecsTile.Type || neighbor.Height != ecsTile.Height);
        }
        
        private static bool TryGetNorthNeighbor(AnonymousBuffer<ECS.Tile> buffer, int index, out ECS.Tile neighbor)
        {
            if (index > Floor.FloorSize * Floor.FloorSize - Floor.FloorSize)
            {
                neighbor = default;
                return false;
            }

            neighbor = buffer.GetAt(index + Floor.FloorSize);
            return true;
        }
        
        private static bool TryGetNorthEastNeighbor(AnonymousBuffer<ECS.Tile> buffer, int index, out ECS.Tile neighbor)
        {
            if (index > Floor.FloorSize * Floor.FloorSize - Floor.FloorSize)
            {
                neighbor = default;
                return false;
            }
            
            if (index % Floor.FloorSize == Floor.FloorSize - 1)
            {
                neighbor = default;
                return false;
            }

            neighbor = buffer.GetAt(index + Floor.FloorSize + 1);
            return true;
        }
        
        private static bool TryGetEastNeighbor(AnonymousBuffer<ECS.Tile> buffer, int index, out ECS.Tile neighbor)
        {
            if (index % Floor.FloorSize == Floor.FloorSize - 1)
            {
                neighbor = default;
                return false;
            }

            neighbor = buffer.GetAt(index + 1);
            return true;
        }
        
        private static bool TryGetSouthEastNeighbor(AnonymousBuffer<ECS.Tile> buffer, int index, out ECS.Tile neighbor)
        {
            if (index < Floor.FloorSize)
            {
                neighbor = default;
                return false;
            }
            
            if (index % Floor.FloorSize == Floor.FloorSize - 1)
            {
                neighbor = default;
                return false;
            }

            neighbor = buffer.GetAt(index + Floor.FloorSize + 1);
            return true;
        }
        
        private static bool TryGetSouthNeighbor(AnonymousBuffer<ECS.Tile> buffer, int index, out ECS.Tile neighbor)
        {
            if (index < Floor.FloorSize)
            {
                neighbor = default;
                return false;
            }

            neighbor = buffer.GetAt(index - Floor.FloorSize);
            return true;
        }
        
        private static bool TryGetSouthWestNeighbor(AnonymousBuffer<ECS.Tile> buffer, int index, out ECS.Tile neighbor)
        {
            if (index < Floor.FloorSize)
            {
                neighbor = default;
                return false;
            }
            if (index % Floor.FloorSize == 0)
            {
                neighbor = default;
                return false;
            }

            neighbor = buffer.GetAt(index + Floor.FloorSize + 1);
            return true;
        }
        
        private static bool TryGetWestNeighbor(AnonymousBuffer<ECS.Tile> buffer, int index, out ECS.Tile neighbor)
        {
            if (index % Floor.FloorSize == 0)
            {
                neighbor = default;
                return false;
            }

            neighbor = buffer.GetAt(index - 1);
            return true;
        }
        
        private static bool TryGetNorthWestNeighbor(AnonymousBuffer<ECS.Tile> buffer, int index, out ECS.Tile neighbor)
        {
            if (index > Floor.FloorSize * Floor.FloorSize - Floor.FloorSize)
            {
                neighbor = default;
                return false;
            }
            
            if (index % Floor.FloorSize == 0)
            {
                neighbor = default;
                return false;
            }

            neighbor = buffer.GetAt(index - 1);
            return true;
        }
    }
}