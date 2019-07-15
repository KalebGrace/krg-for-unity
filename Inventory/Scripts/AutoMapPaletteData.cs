using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace KRG
{
    [System.Serializable]
    public struct AutoMapPaletteData
    {
        //unvisited always starts at 0

        public int visitedStartIndex;

        public TileBase[] tiles;

        public TileBase GetVisitedTile(TileBase currentTile)
        {
            int i = int.Parse(currentTile.name.Split('_')[1]);

            if (i < visitedStartIndex)
            {
                return tiles[i + visitedStartIndex];
            }
            else
            {
                return currentTile;
            }
        }
    }
}
