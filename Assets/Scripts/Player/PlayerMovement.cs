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

    List<Vector2Int> oldBodyPositions;
    List<Vector2Int> newBodyPositions;

    Vector2Int currentPosition = new Vector2Int(0, 0);
    Vector2Int heldDirection = new Vector2Int(0, 0);
    Vector2Int previouslyHeldDirection = new Vector2Int(0, 0);

    float moveHoldTimer;
    float inputCooldown = 0.15f;
    float cooldownTimer = 0f;

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
        
        #region Movement Definitions

        heldDirection = new Vector2Int(0, 0);

        List<Vector2Int> currentBodyPositions = playerBody.GetPixelPositions();

        Vector2Int destinationPosition;
        Vector2Int movement = new Vector2Int(0, 0);

        float scrollInput = Mouse.current.scroll.ReadValue().y;

        #endregion

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        #region Get Held Direction
        
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

        #endregion

        #region Basic Movement Calculation
        
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

        #endregion

        #region Rotation Mechanics
        
        if ((Keyboard.current.eKey.wasPressedThisFrame || scrollInput > 0) && cooldownTimer <= 0)
        {
            oldBodyPositions = new List<Vector2Int>(playerBody.GetPixelPositions());

            playerBody.RotateClockwise();
            cooldownTimer = inputCooldown;

            newBodyPositions = playerBody.GetPixelPositions();

            RotationHit hit = GridPhysics.FindRotationHit(
                oldBodyPositions,
                newBodyPositions,
                currentPosition,
                worldClusters
            );

            GridPhysics.Slap(hit, worldClusters, playerBody.GetWorldPixelPositions());
            
            foreach (WorldCluster cluster in worldClusters)
            {
                if (cluster != null)
                {
                    GridPhysics.FloatClusterToLegalPosition(cluster, worldClusters, playerBody.GetWorldPixelPositions());
                }
            }
        }

        if ((Keyboard.current.qKey.wasPressedThisFrame || scrollInput < 0) && cooldownTimer <= 0)
        {
            oldBodyPositions = new List<Vector2Int>(playerBody.GetPixelPositions());
            
            playerBody.RotateCounterClockwise();
            cooldownTimer = inputCooldown;

            newBodyPositions = playerBody.GetPixelPositions();

            RotationHit hit = GridPhysics.FindRotationHit(
                oldBodyPositions,
                newBodyPositions,
                currentPosition,
                worldClusters
            );

            GridPhysics.Slap(hit, worldClusters, playerBody.GetWorldPixelPositions());

            foreach (WorldCluster cluster in worldClusters)
            {
                if (cluster != null)
                {
                    GridPhysics.FloatClusterToLegalPosition(cluster, worldClusters, playerBody.GetWorldPixelPositions());
                }
            }
        }

        #endregion

        #region Basic Movement
            
            if (movement.x != 0 || movement.y != 0)
            {
                destinationPosition = currentPosition + movement;

                List<WorldCluster> contactedClusters =
                    FindClustersAtDestination(currentBodyPositions, destinationPosition);

                if (contactedClusters.Count > 0)
                {
                    if (Mouse.current.leftButton.isPressed)
                    {
                        WorldCluster contactedCluster = contactedClusters[0];

                        GridPhysics.MoveCluster(contactedCluster, movement, 1, worldClusters, playerBody.GetWorldPixelPositions());
                    }
                    else
                    {
                        foreach (WorldCluster contactedCluster in contactedClusters)
                        {
                            if (playerBody.GetRotationStatus() && contactedCluster.GetMovementStatus())
                            {
                                playerBody.Assimilate(contactedCluster);
                            }
                        }
                    }
                }
                else
                {
                    currentPosition = destinationPosition;

                    transform.position = GridMath.ConvertVector2Int(currentPosition);
                }
            }

        #endregion
        
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

    public void AddWorldCluster(WorldCluster cluster)
    {
        worldClusters.Add(cluster);
    }

    public void RemoveWorldCluster(WorldCluster cluster)
    {
        worldClusters.Remove(cluster);
    }
}