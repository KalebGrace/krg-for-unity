//KRG reference

using UnityEngine;
using UnityEngine.Tilemaps;

namespace KRG
{
    public class AutoMap : MonoBehaviour
    {
        public Vector3 offset = default;

        public Grid grid { get; private set; }

        public Tilemap tilemap { get; private set; }

        public Camera mapCamera { get; private set; }

        public Visibility visibility;

        private Vector3 anchor;

        private AutoMapPaletteData paletteData;

        private AutoMapSaveData saveData;

        public enum Visibility
        {
            None = 0,
            Revealed = 1,
        }

        private void Awake()
        {
            grid = this.Require<Grid>();

            tilemap = G.U.Require(gameObject.GetComponentInChildren<Tilemap>());

            mapCamera = G.U.Require(gameObject.GetComponentInChildren<Camera>());

            anchor = transform.position;

            tilemap.CompressBounds();

            tilemap.color = Color.white;

            G.inv.AutoMapSaveDataRequested += OnAutoMapSaveDataRequested;
            G.inv.AutoMapSaveDataProvided += OnAutoMapSaveDataProvided;
            OnAutoMapSaveDataProvided();

            paletteData = G.config.AutoMapPaletteData;

            RenderAll();
        }

        public void LateUpdate()
        {
            var pc = PlayerCharacter.instance;

            if (pc == null) return;

            var pcPos = pc.transform.position;

            var cp = tilemap.WorldToCell(pcPos + offset);

            Discover(cp);

            var camTF = mapCamera.transform;
            var cpV3 = (Vector3)cp;
            var csz = grid.cellSize;

            cpV3.Scale(csz); //cell to world (snapped to grid)
            cpV3 += anchor;  //if auto-map is not at origin
            cpV3 += csz / 2; //half-cell offset
            cpV3.z = camTF.position.z;
            camTF.position = cpV3;
        }

        private void OnDestroy()
        {
            G.inv.AutoMapSaveDataRequested -= OnAutoMapSaveDataRequested;
            G.inv.AutoMapSaveDataProvided -= OnAutoMapSaveDataProvided;
            OnAutoMapSaveDataRequested();
        }

        public void Discover(Vector3Int cp)
        {
            if (SetDiscovered(cp))
            {
                Render(cp);
            }
        }

        bool SetDiscovered(Vector3Int cp)
        {
            var ai = GetArrayIndices(cp);

            if (IsInBounds(cp) && !saveData.GetIsVisited(ai.x, ai.y))
            {
                saveData.SetIsVisted(ai.x, ai.y, true);
                return true;
            }

            return false;
        }

        bool IsDiscovered(Vector3Int cp)
        {
            var ai = GetArrayIndices(cp);

            return IsInBounds(cp) && saveData.GetIsVisited(ai.x, ai.y);
        }

        bool IsInBounds(Vector3Int cp)
        {
            var ai = GetArrayIndices(cp);

            bool xInBounds = ai.x.Between(0, saveData.Width);
            bool yInBounds = ai.y.Between(0, saveData.Height);

            return xInBounds && yInBounds;
        }

        Vector2Int GetArrayIndices(Vector3Int cp)
        {
            var cb = tilemap.cellBounds;

            int ix = cp.x - cb.min.x;
            int iy = cp.y - cb.min.y;

            return new Vector2Int(ix, iy);
        }

        public void RenderAll()
        {
            var b = tilemap.cellBounds;

            for (int x = b.min.x; x < b.max.x; ++x)
            {
                for (int y = b.min.y; y < b.max.y; ++y)
                {
                    for (int z = b.min.z; z < b.max.z; ++z)
                    {
                        Render(new Vector3Int(x, y, z));
                    }
                }
            }
        }

        public void Render(Vector3Int cp)
        {
            tilemap.SetTileFlags(cp, tilemap.GetTileFlags(cp) & ~TileFlags.LockColor);

            TileBase tile = tilemap.GetTile(cp);

            if (tile == null) return;

            if (IsDiscovered(cp))
            {
                tilemap.SetColor(cp, Color.white);

                tile = paletteData.GetVisitedTile(tile);
                tilemap.SetTile(cp, tile);
            }
            else if (paletteData.IsHiddenArea(tile) || visibility != Visibility.Revealed)
            {
                tilemap.SetColor(cp, Color.clear);
            }
        }

        public void OnAutoMapSaveDataRequested()
        {
            G.inv.SetAutoMapSaveData(saveData);
        }

        public void OnAutoMapSaveDataProvided()
        {
            saveData = G.inv.GetAutoMapSaveData(this);
        }
    }
}
