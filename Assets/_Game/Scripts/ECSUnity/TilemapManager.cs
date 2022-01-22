using System;
using Tofunaut.TofuECS;
using Tofunaut.TofuECS_Rogue.ECS;
using Tofunaut.TofuECS_Rogue.ECSUnity.Tiles;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tofunaut.TofuECS_Rogue.ECSUnity
{
    public class TilemapManager : MonoBehaviour
    {
        [SerializeField] private Grid _grid;
        [SerializeField] private Tilemap _tilemap;

        [Header("Tile Prefabs")]
        [SerializeField] private RogueTileBase _bedRock;
        [SerializeField] private RogueTileBase _stone;
        [SerializeField] private RogueTileBase _ladderDown;
        [SerializeField] private RogueTileBase _ladderUp;
        
        private void Awake()
        {
            SetEnabled(false);
        }

        public void SetEnabled(bool doEnable)
        {
            gameObject.SetActive(doEnable);
            enabled = enabled;
        }

        private RogueTileBase GetTilePrefab(ECS.Tile tile)
        {
            return tile.Type switch
            {
                TileType.Void => null,
                TileType.Bedrock => _bedRock,
                TileType.Stone => _stone,
                TileType.LadderDown => _ladderDown,
                TileType.LadderUp => _ladderUp,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void UpdateTiles(AnonymousBuffer<ECS.Tile> buffer, ECS.Tile[] tiles, int[] indexes)
        {
            for (var i = 0; i < tiles.Length && i < indexes.Length; i++)
            {
                var tilePrefab = GetTilePrefab(tiles[i]);
                if (tilePrefab == null)
                    continue;
                
                var tile = Instantiate(tilePrefab);
                tile.TileBuffer = buffer;
                tile.ECSTileIndex = indexes[i];
                _tilemap.SetTile(new Vector3Int(i % Floor.MaxFloorSize, i / Floor.MaxFloorSize, 0), tile);
            }
        }
    }
}