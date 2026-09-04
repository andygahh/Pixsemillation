using UnityEngine;

public class WorldPixel : MonoBehaviour
{
    Vector2Int currentPosition = new Vector2Int();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentPosition.x = Mathf.RoundToInt(transform.position.x);
        currentPosition.y = Mathf.RoundToInt(transform.position.y);
    }

    public Vector2Int GetGridPosition()
    {
        return currentPosition;
    }

    
}
