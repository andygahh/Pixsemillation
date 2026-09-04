using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    Vector2Int CORE = new Vector2Int(0, 0);

    List<Vector2Int> pixels = new List<Vector2Int>();

    void Start()
    {
        pixels.Add(CORE);
    }

    void Update()
    {
        
    }

    public int GetPixelCount()
    {
        return pixels.Count;
    }

    public void Assimilate(WorldPixel pixel)
    {
        pixel.transform.SetParent(transform, true);
        Vector2Int relativePosition = new Vector2Int(Mathf.RoundToInt(pixel.transform.localPosition.x), Mathf.RoundToInt(pixel.transform.localPosition.y));
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
