using UnityEngine;

namespace KRG
{
    public struct AutoMapSaveData
    {
        public readonly int version;
        public readonly int gameplaySceneId;
        public bool[][] discoveredJagged; //visited

        public int Width { get; private set; }
        public int Height { get; private set; }

        public AutoMapSaveData(int gameplaySceneId, int width, int height)
        {
            version = 1;
            this.gameplaySceneId = gameplaySceneId;

            Width = width;
            Height = height;

            discoveredJagged = new bool[width][];

            for (int i = 0; i < width; ++i)
            {
                discoveredJagged[i] = new bool[height];
            }
        }

        public bool GetIsVisited(int x, int y)
        {
            return discoveredJagged[x][y];
        }

        public void SetIsVisted(int x, int y, bool value)
        {
            discoveredJagged[x][y] = value;
        }

        public void UpdateDimensions(int newWidth, int newHeight)
        {
            if (newWidth > Width || newHeight > Height)
            {
                newWidth = Mathf.Max(newWidth, Width);
                newHeight = Mathf.Max(newHeight, Height);

                var old = discoveredJagged;

                discoveredJagged = new bool[newWidth][];

                for (int i = 0; i < newWidth; ++i)
                {
                    discoveredJagged[i] = new bool[newHeight];

                    if (i < Width)
                    {
                        for (int j = 0; j < Height; ++j)
                        {
                            discoveredJagged[i][j] = old[i][j];
                        }
                    }
                }
            }

            Width = newWidth;
            Height = newHeight;
        }
    }
}
