using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] WorldPixel worldPixel;

    PlayerBody playerBody;

    Vector2Int currentPosition = new Vector2Int(0, 0);
    Vector2Int destinationPosition;

    void Start()
    {
        playerBody = GetComponent<PlayerBody>();
    }

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
            destinationPosition = currentPosition + movement;

            if (worldPixel != null && CheckDestination(destinationPosition, worldPixel.GetGridPosition()))
            {
                playerBody.Assimilate(worldPixel);
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
