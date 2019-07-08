//KRG reference

using UnityEngine;
using UnityEngine.Tilemaps;

namespace KRG
{
    public class AutoMap : MonoBehaviour
    {
        public Color discoveredColor = new Color(255, 133, 234);

        public Color undiscoveredColor = new Color(20, 123, 193);

        public Color hiddenColor = new Color(22, 28, 34);

        public Grid grid { get; private set; }

        public Tilemap tilemap { get; private set; }

        public Camera mapCamera { get; private set; }

        public AutoMapSaveData saveData;

        public Visibility visibility;

        Visibility defaultVisibility;

        public enum Visibility
        {
            None = 0,
            Revealed = 1,
        }

        static AutoMapSaveData[] s_SaveDataArray;

        void Awake()
        {
            grid = this.Require<Grid>();

            tilemap = G.U.Require(gameObject.GetComponentInChildren<Tilemap>());

            mapCamera = G.U.Require(gameObject.GetComponentInChildren<Camera>());

            tilemap.CompressBounds();

            tilemap.color = Color.white;

            defaultVisibility = visibility;

            ResetProgress();

            RenderAll();
        }

        public void ResetProgress()
        {
            var cb = tilemap.cellBounds;

            var n = cb.size.x;
            var m = cb.size.y;

            saveData.discovered = new bool[n, m];

            visibility = defaultVisibility;
        }

        public void LateUpdate()
        {
            var pc = PlayerCharacter.instance;

            if (pc == null) return;

            var pcPos = pc.transform.position;

            var cp = tilemap.WorldToCell(pcPos);

            Discover(cp);

            var camTF = mapCamera.transform;
            var cpV3 = (Vector3)cp;
            var csz = grid.cellSize;

            cpV3.Scale(csz); //cell to world (snapped to grid)
            cpV3 += csz / 2; //half-cell offset
            cpV3.z = camTF.position.z;
            camTF.position = cpV3;
        }

        private void OnDestroy()
        {
            var m = s_SaveDataArray;
            int l = s_SaveDataArray.Length + 1; //rip
            s_SaveDataArray = new AutoMapSaveData[l];
            m.CopyTo(s_SaveDataArray, 0);
            saveData.gameplaySceneId = G.app.GameplaySceneId;
            s_SaveDataArray[l - 1] = saveData;
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

            if (IsInBounds(cp) && !saveData.discovered[ai.x, ai.y])
            {
                saveData.discovered[ai.x, ai.y] = true;
                return true;
            }

            return false;
        }

        bool IsDiscovered(Vector3Int cp)
        {
            var ai = GetArrayIndices(cp);

            return IsInBounds(cp) && saveData.discovered[ai.x, ai.y];
        }

        bool IsInBounds(Vector3Int cp)
        {
            var ai = GetArrayIndices(cp);

            bool xInBounds = ai.x.Between(0, saveData.discovered.GetLength(0));
            bool yInBounds = ai.y.Between(0, saveData.discovered.GetLength(1));

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

            if (IsDiscovered(cp))
            {
                tilemap.SetColor(cp, discoveredColor);
            }
            else if (visibility == Visibility.Revealed)
            {
                tilemap.SetColor(cp, undiscoveredColor);
            }
            else
            {
                tilemap.SetColor(cp, hiddenColor);
            }
        }

        internal static AutoMapSaveData[] GetSaveData()
        {
            return (AutoMapSaveData[])s_SaveDataArray.Clone();
            //TODO: just make s_SaveDataArray into s_SaveDataDictionary
        }

        internal static void SetSaveData(AutoMapSaveData[] maps)
        {
            s_SaveDataArray = maps;
            //TODO: load it out of here
        }
    }
}
