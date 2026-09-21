using System.Collections.Generic;
using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    [SerializeField] float rotationSpeed;

    PlayerMovement playerMovement;

    Vector2Int corePosition = new Vector2Int(0, 0);

    List<Vector2Int> pixels = new List<Vector2Int>();

    float currentAngle;
    float targetRotationAngle;

    bool isVisualRotationComplete;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();

        targetRotationAngle = transform.eulerAngles.z;

        pixels.Add(corePosition);
    }

    void LateUpdate()
    {
        currentAngle = transform.eulerAngles.z;

        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetRotationAngle, rotationSpeed * Time.deltaTime);

        DoRotation(newAngle);

        float angleDiff = Mathf.DeltaAngle(newAngle, targetRotationAngle);

        if (Mathf.Abs(angleDiff) <= 0.01f)
        {
            isVisualRotationComplete = true;
        }
        else
        {
            isVisualRotationComplete = false;
        }
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
        Vector2Int corePositionWorldPosition = GridMath.ConvertVector3(transform.position);

        foreach (Vector2Int clusterWorldPosition in clusterWorldPositions)
        {
            Vector2Int relativePosition = clusterWorldPosition - corePositionWorldPosition;

            pixels.Add(relativePosition);
        }

        foreach (Pixel child in cluster.GetComponentsInChildren<Pixel>())
        {
            child.transform.SetParent(transform, true);
        }

        playerMovement.RemoveWorldCluster(cluster);
        
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

    public void DoRotation (float newAngle)
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, newAngle);
    }

    public void RemovePixels(List<Vector2Int> positionsToRemove)
    {
        foreach (Vector2Int pixel in positionsToRemove)
        {
            pixels.Remove(pixel);
        }
    }

    public bool GetRotationStatus ()
    {
        return isVisualRotationComplete;
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
