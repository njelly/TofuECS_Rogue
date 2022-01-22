using System;
using Tofunaut.TofuECS;
using Tofunaut.TofuECS_Rogue.ECS;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tofunaut.TofuECS_Rogue.ECSUnity
{
    public class TilemapManager : MonoBehaviour
    {
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private RogueTile _rogueTile;
        
        private void Awake()
        {
            SetEnabled(false);
        }

        public void SetEnabled(bool doEnable)
        {
            gameObject.SetActive(doEnable);
            enabled = enabled;
        }

        public void UpdateTiles(AnonymousBuffer<ECS.Tile> buffer, ECS.Tile[] tiles, int[] indexes)
        {
            _rogueTile.Buffer = buffer;
            for (var i = 0; i < tiles.Length && i < indexes.Length; i++)
            {
                var x = indexes[i] % Floor.FloorSize;
                var y = indexes[i] / Floor.FloorSize;
                _tilemap.SetTile(new Vector3Int(x, y, 0), _rogueTile);
            }
        }
    }
}