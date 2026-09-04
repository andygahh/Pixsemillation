using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] WorldPixel worldPixel;
    [SerializeField] List<WorldPixel> worldPixels;

    PlayerBody playerBody;

    Vector2Int currentPosition = new Vector2Int(0, 0);
    Vector2Int destinationPosition;

    void Start()
    {
        playerBody = GetComponent<PlayerBody>();
    }

    void Update()
    {
        List<Vector2Int> currentBodyPositions = playerBody.GetPixelPositions();

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

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            playerBody.RotateClockwise();
        }

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            playerBody.RotateCounterClockwise();
        }

        if (movement.x != 0 || movement.y != 0)
        {
            destinationPosition = currentPosition + movement;
            bool madeContact = false;

            if (worldPixels.Count != 0)
            {
                foreach (WorldPixel worldPixel in worldPixels)
                {
                    if (worldPixel == null)
                    {
                        continue;
                    }

                    foreach (Vector2Int pixel in currentBodyPositions)
                    {
                        Debug.Log(pixel);
                        
                        Vector2Int proposedPosition = destinationPosition + pixel;

                        if (CheckDestination(proposedPosition, worldPixel.GetGridPosition()))
                        {
                            playerBody.Assimilate(worldPixel);
                            madeContact = true;
                            break;
                        }
                    }

                    if (madeContact)
                    {
                        break;
                    }
                }
            }
            
            if (madeContact)
            {
                transform.position = new Vector3(currentPosition.x, currentPosition.y, 0);
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
