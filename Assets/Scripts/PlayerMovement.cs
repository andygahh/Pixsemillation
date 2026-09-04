using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] WorldPixel worldPixel;

    Vector2Int currentPosition = new Vector2Int(0, 0);
    public static int pixelCount = 1;

    // Update is called once per frame
    void Update()
    {
        Vector2Int movement = new Vector2Int(0, 0);

        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            movement.y += 1;
        }

        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            movement.y -= 1;
        }

        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            movement.x -= 1;
        }

        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            movement.x += 1;
        }

        if (movement.x != 0 || movement.y != 0)
        {
            Vector2Int destinationPosition = currentPosition + movement;

            if (worldPixel != null)
            {
                if (CheckDestination(destinationPosition, worldPixel.GetGridPosition()))
                {
                    pixelCount += 1;
                    worldPixel.transform.SetParent(transform, true);
                    Destroy(worldPixel);
                }
                else
                {
                    currentPosition = destinationPosition;
                    transform.position = new Vector3(currentPosition.x, currentPosition.y, 0);
                }
            }
            else
            {
                currentPosition = destinationPosition;
                transform.position = new Vector3(currentPosition.x, currentPosition.y, 0);
            }
            
        }

    }

    private bool CheckDestination(Vector2Int player, Vector2Int pixel)
    {
        return player == pixel;
    }
}
