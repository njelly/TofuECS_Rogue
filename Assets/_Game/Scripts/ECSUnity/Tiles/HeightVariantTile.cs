using System;
using UnityEngine;

namespace Tofunaut.TofuECS_Rogue.ECSUnity.Tiles
{
    [CreateAssetMenu(menuName = "TofuECS_Rogue/HeightVariantTile")]
    public class HeightVariantTile : RogueTileBase
    {
        [Serializable]
        public struct HeightTier
        {
            public Sprite EdgeVariant;
            public Sprite Sprite;
            public int MinHeight;
        }
        
        public Sprite DefaultSprite;
        public HeightTier[] HeightTiers;

        protected override Sprite PickSprite()
        {
            var tile = TileBuffer.GetAt(ECSTileIndex);

            var indexToUse = -1;
            var tallestValidHeight = int.MinValue;
            for (var i = 0; i < HeightTiers.Length; i++)
            {
                if (HeightTiers[i].MinHeight > tile.Height || HeightTiers[i].MinHeight <= tallestValidHeight) 
                    continue;
                
                indexToUse = i;
                tallestValidHeight = HeightTiers[i].MinHeight;
            }

            if (indexToUse < 0)
                return DefaultSprite;
            
            if (HeightTiers[indexToUse].EdgeVariant != null && IsOnEdge())
                return HeightTiers[indexToUse].EdgeVariant;

            return HeightTiers[indexToUse].Sprite;
        }
    }
}