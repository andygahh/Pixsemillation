using System.Collections.Generic;
using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    Vector2Int corePosition = new Vector2Int(0, 0);

    List<Vector2Int> pixels = new List<Vector2Int>();

    void Start()
    {
        pixels.Add(corePosition);
    }

    public int GetPixelCount()
    {
        return pixels.Count;
    }

    public void Assimilate(WorldPixel pixel)
    {
        Vector2Int pixelWorldPosition = pixel.GetGridPosition();
        Vector2Int corePositionWorldPosition = new Vector2Int();

        corePositionWorldPosition.x = Mathf.RoundToInt(transform.position.x);
        corePositionWorldPosition.y = Mathf.RoundToInt(transform.position.y);
        
        Vector2Int relativePosition = pixelWorldPosition - corePositionWorldPosition;
        pixel.transform.SetParent(transform, true);
        pixels.Add(relativePosition);
        Destroy(pixel);
    }

    public List<Vector2Int> GetPixelPositions()
    {
        return pixels;
    }

    public void RotateClockwise()
    {
        transform.Rotate(0, 0, -90);

        for (int i = 0; i < pixels.Count; i++)
        {
            Vector2Int oldPosition = pixels[i];

            Vector2Int rotatedPosition = new Vector2Int();
            rotatedPosition.x = oldPosition.y;
            rotatedPosition.y = -oldPosition.x;

            pixels[i] = rotatedPosition;
        }
    }

    public void RotateCounterClockwise()
    {
        transform.Rotate(0, 0, 90);

        for (int i = 0; i < pixels.Count; i++)
        {
            Vector2Int oldPosition = pixels[i];

            Vector2Int rotatedPosition = new Vector2Int();
            rotatedPosition.x = -oldPosition.y;
            rotatedPosition.y = oldPosition.x;

            pixels[i] = rotatedPosition;
        }
    }
}
