using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] List<WorldCluster> worldClusters;
    [SerializeField] float initialMoveDelay;
    [SerializeField] float moveRepeatInterval;

    PlayerBody playerBody;
    DropMode dropMode;

    Vector2Int currentPosition = new Vector2Int(0, 0);
    Vector2Int heldDirection = new Vector2Int(0, 0);
    Vector2Int previouslyHeldDirection = new Vector2Int(0, 0);
    float moveHoldTimer;
    bool isRepeatingMovement;


    void Start()
    {
        playerBody = GetComponent<PlayerBody>();
        dropMode = GetComponent<DropMode>();
    }

    void Update()
    {
        if (dropMode.GetDropModeStatus())
        {
            return;
        }
        else
        {
            heldDirection = new Vector2Int(0, 0);

            List<Vector2Int> currentBodyPositions = playerBody.GetPixelPositions();

            Vector2Int destinationPosition;
            Vector2Int movement = new Vector2Int(0, 0);

            if (Keyboard.current.wKey.isPressed)
            {
                heldDirection.y += 1;
            }

            if (Keyboard.current.sKey.isPressed)
            {
                heldDirection.y -= 1;
            }

            if (Keyboard.current.aKey.isPressed)
            {
                heldDirection.x -= 1;
            }

            if (Keyboard.current.dKey.isPressed)
            {
                heldDirection.x += 1;
            }

            if (heldDirection != previouslyHeldDirection)
            {
                movement = heldDirection;
                moveHoldTimer = 0;
                isRepeatingMovement = false;
                previouslyHeldDirection = heldDirection;
            }

            if (heldDirection != Vector2Int.zero)
            {
                moveHoldTimer += Time.deltaTime;

                if (!isRepeatingMovement)
                {
                    if (moveHoldTimer >= initialMoveDelay)
                    {
                        movement = heldDirection;
                        moveHoldTimer = 0;
                        isRepeatingMovement = true;
                    }
                }
                else
                {
                    if (moveHoldTimer >= moveRepeatInterval)
                    {
                        movement = heldDirection;
                        moveHoldTimer = 0;
                    }
                }
            }
            else
            {
                moveHoldTimer = 0;
                isRepeatingMovement = false;
            }


            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                playerBody.RotateClockwise();
                CheckForAssimilation(currentBodyPositions);

                Debug.Log("Rotated");
            }

            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                playerBody.RotateCounterClockwise();
                CheckForAssimilation(currentBodyPositions);

                Debug.Log("Rotated");
            }

            if (movement.x != 0 || movement.y != 0)
            {
                destinationPosition = currentPosition + movement;

                List<WorldCluster> contactedClusters =
                    FindClustersAtDestination(currentBodyPositions, destinationPosition);

                if (contactedClusters.Count > 0)
                {
                    foreach (WorldCluster contactedCluster in contactedClusters)
                    {
                        playerBody.Assimilate(contactedCluster);
                    }
                }
                else
                {
                    currentPosition = destinationPosition;

                    transform.position = new Vector3(
                        currentPosition.x,
                        currentPosition.y,
                        0
                    );
                }
            }
        }
    }

    private List<WorldCluster> FindClustersAtDestination(
        List<Vector2Int> currentBodyPositions,
        Vector2Int destinationPosition)
    {
        List<WorldCluster> contactedClusters = new List<WorldCluster>();

        foreach (WorldCluster worldCluster in worldClusters)
        {
            if (worldCluster == null)
            {
                continue;
            }

            List<Vector2Int> clusterWorldPositions =
                worldCluster.GetWorldPixelPositions();

            bool clusterContacted = false;

            foreach (Vector2Int pixel in currentBodyPositions)
            {
                Vector2Int proposedBodyPosition =
                    destinationPosition + pixel;

                foreach (Vector2Int clusterPixel in clusterWorldPositions)
                {
                    if (proposedBodyPosition == clusterPixel)
                    {
                        contactedClusters.Add(worldCluster);
                        clusterContacted = true;
                        break;
                    }
                }

                if (clusterContacted)
                {
                    break;
                }
            }
        }

        return contactedClusters;
    }

    private void CheckForAssimilation(List<Vector2Int> currentBodyPositions)
{
    List<WorldCluster> contactedClusters = new List<WorldCluster>();

    foreach (WorldCluster worldCluster in worldClusters)
    {
        if (worldCluster == null)
        {
            continue;
        }

        List<Vector2Int> clusterWorldPositions =
            worldCluster.GetWorldPixelPositions();

        bool clusterContacted = false;

        foreach (Vector2Int pixel in currentBodyPositions)
        {
            Vector2Int bodyCellPosition =
                currentPosition + pixel;

            foreach (Vector2Int clusterPixel in clusterWorldPositions)
            {
                int differenceX = clusterPixel.x - bodyCellPosition.x;
                int differenceY = clusterPixel.y - bodyCellPosition.y;

                if (Mathf.Abs(differenceX) <= 1 &&
                    Mathf.Abs(differenceY) <= 1 &&
                    bodyCellPosition != clusterPixel)
                {
                    contactedClusters.Add(worldCluster);
                    clusterContacted = true;
                    break;
                }
            }

            if (clusterContacted)
            {
                break;
            }
        }
    }

    foreach (WorldCluster contactedCluster in contactedClusters)
    {
        playerBody.Assimilate(contactedCluster);
    }
}

    public void AddWorldCluster(WorldCluster cluster)
    {
        worldClusters.Add(cluster);
    }
}