using System.Collections.Generic;
using UnityEngine;

public class TestClusterSpawner : MonoBehaviour
{
    [SerializeField] private Pixel pixelPrefab;
    [SerializeField] private PlayerMovement playerMovement;

    private readonly List<Vector2Int> occupiedPositions = new List<Vector2Int>();

    private const int MAX_NUMBER_OF_CLUSTERS = 50;
    private const int MAX_CLUSTER_SIZE = 5;

    private const int CEILING = 40;
    private const int FLOOR = -40;

    private const int MAX_PLACEMENT_ATTEMPTS = 100;

    private readonly Vector2Int[] adjacents =
    {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),

        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, -1)
    };

    private void Start()
    {
        for (int i = 0; i < MAX_NUMBER_OF_CLUSTERS; i++)
        {
            AssembleCluster();
        }
    }

    private void AssembleCluster()
    {
        Vector2Int clusterPosition = FindOpenStartingPosition();

        GameObject clusterObject = new GameObject("WorldCluster");
        clusterObject.transform.position =
            GridMath.ConvertVector2Int(clusterPosition);

        List<Vector2Int> localPixelPositions = new List<Vector2Int>();

        CreatePixel(clusterObject.transform, Vector2Int.zero);

        localPixelPositions.Add(Vector2Int.zero);
        occupiedPositions.Add(clusterPosition);

        int clusterSize = Random.Range(1, MAX_CLUSTER_SIZE + 1);

        while (localPixelPositions.Count < clusterSize)
        {
            bool pixelPlaced = false;

            for (int attempt = 0; attempt < MAX_PLACEMENT_ATTEMPTS; attempt++)
            {
                Vector2Int anchor =
                    localPixelPositions[Random.Range(0, localPixelPositions.Count)];

                Vector2Int direction =
                    adjacents[Random.Range(0, adjacents.Length)];

                Vector2Int candidateLocalPosition = anchor + direction;
                Vector2Int candidateWorldPosition =
                    clusterPosition + candidateLocalPosition;

                if (occupiedPositions.Contains(candidateWorldPosition))
                {
                    continue;
                }

                CreatePixel(clusterObject.transform, candidateLocalPosition);

                localPixelPositions.Add(candidateLocalPosition);
                occupiedPositions.Add(candidateWorldPosition);

                pixelPlaced = true;
                break;
            }

            if (!pixelPlaced)
            {
                break;
            }
        }

        WorldCluster cluster = clusterObject.AddComponent<WorldCluster>();

        playerMovement.AddWorldCluster(cluster);
    }

    private Vector2Int FindOpenStartingPosition()
    {
        Vector2Int position;

        do
        {
            int x = Random.Range(FLOOR, CEILING + 1);
            int y = Random.Range(FLOOR, CEILING + 1);

            position = new Vector2Int(x, y);
        }
        while (occupiedPositions.Contains(position));

        return position;
    }

    private void CreatePixel(Transform clusterTransform, Vector2Int localPosition)
    {
        Pixel newPixel = Instantiate(pixelPrefab, clusterTransform);

        newPixel.transform.localPosition =
            new Vector3(localPosition.x, localPosition.y, 0);
    }
}