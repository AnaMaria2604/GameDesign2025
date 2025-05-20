using UnityEngine;
using System.Collections.Generic;

public class PathManager : MonoBehaviour
{
        public static PathManager Instance;
        public List<Transform> pathSquares = new List<Transform>();

    public List<Transform> finalPath_TopLeft = new List<Transform>();
    public List<Transform> finalPath_TopRight = new List<Transform>();
    public List<Transform> finalPath_BottomRight = new List<Transform>();
    public List<Transform> finalPath_BottomLeft = new List<Transform>();

    public List<Transform> GetFinalPathForPlayer(int playerIndex)
    {
        switch (playerIndex)
        {
            case 0: return finalPath_TopLeft;
            case 1: return finalPath_TopRight;
            case 2: return finalPath_BottomRight;
            case 3: return finalPath_BottomLeft;
            default: return null;
        }
    }


    private void Awake()
        {
                Instance = this;
        }

        public Transform GetSquareAt(int index)
        {
                int count = pathSquares.Count;
                if (count == 0) return null;

                // te învârți dacă treci de capăt
                int wrappedIndex = index % count;
                return pathSquares[wrappedIndex];
        }


        public int GetPathLength()
        {
                return pathSquares.Count;
        }
}
