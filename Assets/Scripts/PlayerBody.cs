using System.Collections.Generic;
using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    [SerializeField] float rotationSpeed;

    Vector2Int corePosition = new Vector2Int(0, 0);

    List<Vector2Int> pixels = new List<Vector2Int>();

    float currentAngle;
    float targetRotationAngle;

    void Start()
    {
        targetRotationAngle = transform.eulerAngles.z;

        pixels.Add(corePosition);
    }

    void LateUpdate()
    {
        currentAngle = transform.eulerAngles.z;

        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetRotationAngle, rotationSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, newAngle);
    }

    public int GetPixelCount()
    {
        return pixels.Count;
    }

    public void Assimilate(WorldCluster cluster)
    {
        Debug.Log("Before Assimilation");
        DebugPixelPositions();

        List<Vector2Int> clusterWorldPositions = cluster.GetWorldPixelPositions();
        Vector2Int corePositionWorldPosition = new Vector2Int();

        corePositionWorldPosition.x = Mathf.RoundToInt(transform.position.x);
        corePositionWorldPosition.y = Mathf.RoundToInt(transform.position.y);

        foreach (Vector2Int clusterWorldPosition in clusterWorldPositions)
        {
            Vector2Int relativePosition = clusterWorldPosition - corePositionWorldPosition;

            pixels.Add(relativePosition);
        }

        foreach (Pixel child in cluster.GetComponentsInChildren<Pixel>())
        {
            child.transform.SetParent(transform, true);
        }
        
        Destroy(cluster.gameObject);

        Debug.Log("After Assimilation");
        DebugPixelPositions();
    }

    public List<Vector2Int> GetPixelPositions()
    {
        return pixels;
    }

    public void RotateClockwise()
    {
        for (int i = 0; i < pixels.Count; i++)
        {
            Vector2Int oldPosition = pixels[i];

            Vector2Int rotatedPosition = new Vector2Int();
            rotatedPosition.x = oldPosition.y;
            rotatedPosition.y = -oldPosition.x;

            pixels[i] = rotatedPosition;
        }

        targetRotationAngle -= 90;
    }

    public void RotateCounterClockwise()
    {
        for (int i = 0; i < pixels.Count; i++)
        {
            Vector2Int oldPosition = pixels[i];

            Vector2Int rotatedPosition = new Vector2Int();
            rotatedPosition.x = -oldPosition.y;
            rotatedPosition.y = oldPosition.x;

            pixels[i] = rotatedPosition;
        }

        targetRotationAngle += 90;
    }

    public void RemovePixels(List<Vector2Int> positionsToRemove)
    {
        foreach (Vector2Int pixel in positionsToRemove)
        {
            pixels.Remove(pixel);
        }
    }

    public void DebugPixelPositions()
    {
        HashSet<Vector2Int> uniquePositions = new HashSet<Vector2Int>();

        foreach (Vector2Int pixel in pixels)
        {
            if (!uniquePositions.Add(pixel))
            {
                Debug.LogWarning("DUPLICATE POSITION: " + pixel);
            }
        }

        Debug.Log("Total pixels: " + pixels.Count);
        Debug.Log("Unique positions: " + uniquePositions.Count);
    }
}
