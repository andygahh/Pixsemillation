using System.Collections.Generic;
using UnityEngine;

public class WorldCluster : MonoBehaviour
{
    HashSet<Vector2Int> localPixelPositions = new HashSet<Vector2Int>();

    public int Mass => localPixelPositions.Count;

    void Start()
    {
        RefreshCluster();
    }

    public List<Vector2Int> GetPixelPositions()
    {
        return new List<Vector2Int>(localPixelPositions);
    }

    public List<Vector2Int> GetWorldPixelPositions()
    {
        List<Vector2Int> worldPositions = new List<Vector2Int>();

        Vector2Int currentAnchor = GridMath.ConvertVector3(transform.position);

        foreach (Vector2Int position in localPixelPositions)
        {
            worldPositions.Add(position + currentAnchor);
        }

        return worldPositions;
    }

    public void RefreshCluster()
    {
        localPixelPositions.Clear();

        foreach (Transform child in transform)
        {
            Vector2Int currentPosition = GridMath.ConvertVector3(child.localPosition);

            localPixelPositions.Add(currentPosition);
        }
    }
}
