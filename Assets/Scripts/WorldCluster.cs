using System.Collections.Generic;
using UnityEngine;

public class WorldCluster : MonoBehaviour
{
    List<Vector2Int> localPixelPositions = new List<Vector2Int>();

    void Start()
    {
        foreach (Transform child in transform)
        {
            Vector2Int currentPosition = GridMath.ConvertVector3(child.localPosition);

            localPixelPositions.Add(currentPosition);
        }
    }

    public List<Vector2Int> GetPixelPositions()
    {
        return localPixelPositions;
    }

    public List<Vector2Int> GetWorldPixelPositions()
    {
        List<Vector2Int> worldPositions = new List<Vector2Int>();

        Vector2Int currentAnchor = GridMath.ConvertVector3(transform.position);

        for (int i = 0; i < localPixelPositions.Count; i++)
        {
            worldPositions.Add(localPixelPositions[i] + currentAnchor);
        }

        return worldPositions;
    }
}
