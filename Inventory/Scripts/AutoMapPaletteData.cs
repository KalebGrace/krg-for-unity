using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace KRG
{
    [System.Serializable]
    public struct AutoMapPaletteData
    {
        public TileBase[] normalTiles;
        public TileBase[] visHidTiles;

        public TileBase GetVisitedTile(TileBase currentTile)
        {
            for (int i = 0; i < normalTiles.Length; ++i)
            {
                if (currentTile.name == normalTiles[i].name)
                {
                    return visHidTiles[i];
                }
            }
            return currentTile;
        }

        public bool IsHiddenArea(TileBase currentTile)
        {
            for (int i = 0; i < visHidTiles.Length; ++i)
            {
                if (currentTile.name == visHidTiles[i].name)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
