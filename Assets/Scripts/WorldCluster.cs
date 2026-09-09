using System.Collections.Generic;
using UnityEngine;

public class WorldCluster : MonoBehaviour
{
    Vector2Int worldAnchor = new Vector2Int();
    List<Vector2Int> localPixelPositions = new List<Vector2Int>();

    void Start()
    {
        worldAnchor.x = Mathf.RoundToInt(transform.position.x);
        worldAnchor.y = Mathf.RoundToInt(transform.position.y);

        foreach (Transform child in transform)
        {
            Vector2Int currentPosition = new Vector2Int();

            currentPosition.x = Mathf.RoundToInt(child.localPosition.x);
            currentPosition.y = Mathf.RoundToInt(child.localPosition.y);

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

        for (int i = 0; i < localPixelPositions.Count; i++)
        {
            worldPositions.Add(localPixelPositions[i] + worldAnchor);
        }

        return worldPositions;
    }
}
