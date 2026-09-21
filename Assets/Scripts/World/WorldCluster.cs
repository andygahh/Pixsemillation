using System.Collections.Generic;
using UnityEngine;

public class WorldCluster : MonoBehaviour
{
    [SerializeField] float movementSpeed = 10f;

    HashSet<Vector2Int> localPixelPositions = new HashSet<Vector2Int>();

    public int Mass => localPixelPositions.Count;

    public Vector2Int logicalWorldPosition;

    void Start()
    {
        logicalWorldPosition = GridMath.ConvertVector3(transform.position);
        RefreshCluster();
    }

    void LateUpdate()
    {
        DoAnimation();
    }

    public List<Vector2Int> GetPixelPositions()
    {
        return new List<Vector2Int>(localPixelPositions);
    }

    public List<Vector2Int> GetWorldPixelPositions()
    {
        List<Vector2Int> worldPositions = new List<Vector2Int>();

        foreach (Vector2Int position in localPixelPositions)
        {
            worldPositions.Add(position + logicalWorldPosition);
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

    private void DoAnimation()
    {
        transform.position = Vector3.MoveTowards(transform.position, GridMath.ConvertVector2Int(logicalWorldPosition), movementSpeed * Time.deltaTime);
    }

    public bool GetMovementStatus()
    {
        if (transform.position == GridMath.ConvertVector2Int(logicalWorldPosition))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
