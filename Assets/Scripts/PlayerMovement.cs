using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] List<WorldCluster> worldClusters;

    PlayerBody playerBody;

    Vector2Int currentPosition = new Vector2Int(0, 0);
    

    void Start()
    {
        playerBody = GetComponent<PlayerBody>();
    }

    void Update()
    {
        List<Vector2Int> currentBodyPositions = playerBody.GetPixelPositions();

        Vector2Int destinationPosition;
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
            CheckForAssimilation(currentBodyPositions);
        }

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            playerBody.RotateCounterClockwise();
            CheckForAssimilation(currentBodyPositions);
        }

        if (movement.x != 0 || movement.y != 0)
        {
            destinationPosition = currentPosition + movement;
            
            bool wouldOverlap = CheckCollision(currentBodyPositions, destinationPosition);

            if (!wouldOverlap)
            {
                currentPosition = destinationPosition;
                transform.position = new Vector3(currentPosition.x, currentPosition.y, 0);

                CheckForAssimilation(currentBodyPositions);
            }
        }
    }

    private bool CheckAdjacent(Vector2Int bodyCell, Vector2Int pixel)
    {
        int differenceX = pixel.x - bodyCell.x;
        int differenceY = pixel.y - bodyCell.y;

        if (Mathf.Abs(differenceX) <= 1 && Mathf.Abs(differenceY) <= 1 && bodyCell != pixel)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CheckCollision(List<Vector2Int> currentBodyPositions, Vector2Int destinationPosition)
    {
        bool wouldOverlap = false;

        foreach (WorldCluster worldCluster in worldClusters)
        {
            if (worldCluster == null)
            {
                continue;
            }
            
            List<Vector2Int> clusterWorldPositions = worldCluster.GetWorldPixelPositions();

            foreach (Vector2Int pixel in currentBodyPositions)
            {
                Vector2Int proposedPosition = destinationPosition + pixel;

                foreach (Vector2Int clusterPixel in clusterWorldPositions)
                {
                    if (proposedPosition == clusterPixel)
                    {
                        wouldOverlap = true;
                        break;
                    }
                }
            }
        }

        if (wouldOverlap)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void CheckForAssimilation(List<Vector2Int> currentBodyPositions)
    {
        bool madeContact = false;

        foreach (WorldCluster worldCluster in worldClusters)
        {
            if (worldCluster == null)
            {
                continue;
            }

            List<Vector2Int> clusterWorldPositions = worldCluster.GetWorldPixelPositions();

            foreach (Vector2Int pixel in currentBodyPositions)
            {
                Vector2Int bodyCellPosition = currentPosition + pixel;

                foreach (Vector2Int clusterPixel in clusterWorldPositions)
                {
                    if (CheckAdjacent(bodyCellPosition, clusterPixel))
                    {
                        playerBody.Assimilate(worldCluster);
                        madeContact = true;
                        break;
                    }
                }

                if (madeContact)
                {
                    break;
                }
            }

            if (madeContact)
            {
                break;
            }
        }
    }
}
