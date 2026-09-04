using Unity.VisualScripting;
using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    int pixelCount = 1;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public int GetPixelCount()
    {
        return pixelCount;
    }

    public void Assimilate(WorldPixel pixel)
    {
        pixelCount += 1;
        pixel.transform.SetParent(transform, true);
        Destroy(pixel);
    }

    
}
