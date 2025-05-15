using UnityEngine;
using System.Collections.Generic;

public class PathManager : MonoBehaviour
{
        public static PathManager Instance;
        public List<Transform> pathSquares = new List<Transform>();

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
