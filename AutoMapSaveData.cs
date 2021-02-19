using UnityEngine;

namespace _0G.Legacy
{
    public struct AutoMapSaveData
    {
        public readonly int version;
        public readonly int gameplaySceneId;
        public bool[][] visited;

        public int Width => visited?.Length ?? 0;
        public int Height => Width > 0 ? visited[0].Length : 0;

        public AutoMapSaveData(int gameplaySceneId, int width, int height)
        {
            version = 1;

            this.gameplaySceneId = gameplaySceneId;

            visited = new bool[width][];
            for (int i = 0; i < width; ++i)
            {
                visited[i] = new bool[height];
            }
        }

        public bool IsVisited(int x, int y)
        {
            return visited[x][y];
        }

        public void Visit(int x, int y, bool value = true)
        {
            visited[x][y] = value;
        }

        public void UpdateDimensions(int newWidth, int newHeight)
        {
            int curWidth = Width;
            int curHeight = Height;

            if (newWidth > curWidth || newHeight > curHeight)
            {
                newWidth = Mathf.Max(newWidth, curWidth);
                newHeight = Mathf.Max(newHeight, curHeight);

                bool[][] oldVisited = visited;

                visited = new bool[newWidth][];

                for (int i = 0; i < newWidth; ++i)
                {
                    visited[i] = new bool[newHeight];

                    if (i < curWidth)
                    {
                        for (int j = 0; j < curHeight; ++j)
                        {
                            visited[i][j] = oldVisited[i][j];
                        }
                    }
                }
            }
        }
    }
}