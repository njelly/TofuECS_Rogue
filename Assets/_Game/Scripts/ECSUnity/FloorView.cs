using Tofunaut.TofuECS_Rogue.ECS;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tofunaut.TofuECS_Rogue.ECSUnity
{
    public class FloorView
    {
        private readonly Tilemap _tilemap;
        private readonly RogueTile _rogueTile;

        public FloorView(RogueTile rogueTile)
        {
            _rogueTile = rogueTile;
            
            var gridGo = new GameObject("Grid", typeof(Grid));
            _tilemap = new GameObject("Tilemap", typeof(Tilemap), typeof(TilemapRenderer)).GetComponent<Tilemap>();
            _tilemap.transform.SetParent(gridGo.transform, false);
        }

        public void UpdateTile(int x, int y, in ECS.Tile tile)
        {
            _rogueTile.tile = tile;
            _tilemap.SetTile(new Vector3Int(x, y, 0), _rogueTile);
        }

        public void Destroy()
        {
            Object.Destroy(_tilemap.transform.root.gameObject);
        }
    }
}