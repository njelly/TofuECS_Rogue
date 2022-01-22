using UnityEngine;

namespace Tofunaut.TofuECS_Rogue.ECSUnity.Tiles
{
    [CreateAssetMenu(menuName = "TofuECS_Rogue/StaticTile")]
    public class StaticTile : RogueTileBase
    {
        public Sprite Sprite;

        protected override Sprite PickSprite() => Sprite;
    }
}