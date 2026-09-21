using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClusterPhysicsTester : MonoBehaviour
{
    [SerializeField] GameObject pixelPrefab;

    List<WorldCluster> worldClusters = new List<WorldCluster>();

    WorldCluster moveTest;

    WorldCluster pushA;
    WorldCluster pushB;
    WorldCluster pushC;

    WorldCluster mergeA;
    WorldCluster mergeB;

    int testIndex = 0;

    void Start()
    {
        BuildTests();
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            RunNextTest();
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            ResetTests();
        }
    }

    void BuildTests()
    {
        // TEST 1:
        // Simple logical movement + visual catch-up.
        moveTest = CreateCluster(
            "Move Test",
            new Vector2Int(-10, 6),
            2
        );

        // TEST 2:
        // Recursive push chain.
        pushA = CreateCluster(
            "Push A - Mass 5",
            new Vector2Int(-10, 0),
            5
        );

        pushB = CreateCluster(
            "Push B - Mass 3",
            new Vector2Int(-6, 0),
            3
        );

        pushC = CreateCluster(
            "Push C - Mass 2",
            new Vector2Int(-3, 0),
            2
        );

        // TEST 3:
        // Smaller cluster merges into larger one.
        mergeA = CreateCluster(
            "Merge A - Mass 2",
            new Vector2Int(4, 4),
            2
        );

        mergeB = CreateCluster(
            "Merge B - Mass 5",
            new Vector2Int(7, 4),
            5
        );
    }

    WorldCluster CreateCluster(
        string clusterName,
        Vector2Int anchor,
        int mass)
    {
        GameObject root = new GameObject(clusterName);

        root.transform.position =
            GridMath.ConvertVector2Int(anchor);

        for (int i = 0; i < mass; i++)
        {
            GameObject pixel =
                Instantiate(pixelPrefab, root.transform);

            pixel.transform.localPosition =
                GridMath.ConvertVector2Int(
                    new Vector2Int(0, i)
                );
        }

        WorldCluster cluster =
            root.AddComponent<WorldCluster>();

        worldClusters.Add(cluster);

        return cluster;
    }

    void RunNextTest()
    {
        switch (testIndex)
        {
            case 0:
                Debug.Log("=== TEST 1: SIMPLE MOVEMENT ===");

                LogCluster("BEFORE", moveTest);

                GridPhysics.MoveCluster(
                    moveTest,
                    Vector2Int.right,
                    5,
                    worldClusters,
                    new HashSet<Vector2Int>()
                );

                LogCluster("AFTER LOGICAL MOVE", moveTest);

                break;

            case 1:
                Debug.Log("=== TEST 2: RECURSIVE PUSH ===");

                LogCluster("A BEFORE", pushA);
                LogCluster("B BEFORE", pushB);
                LogCluster("C BEFORE", pushC);

                GridPhysics.MoveCluster(
                    pushA,
                    Vector2Int.right,
                    8,
                    worldClusters,
                    new HashSet<Vector2Int>()
                );

                LogCluster("A AFTER", pushA);
                LogCluster("B AFTER", pushB);
                LogCluster("C AFTER", pushC);

                break;

            case 2:
                Debug.Log("=== TEST 3: MERGE ===");

                LogCluster("SMALL BEFORE", mergeA);
                LogCluster("LARGE BEFORE", mergeB);

                GridPhysics.MoveCluster(
                    mergeA,
                    Vector2Int.right,
                    5,
                    worldClusters,
                    new HashSet<Vector2Int>()
                );

                if (mergeB != null)
                {
                    LogCluster("LARGE AFTER", mergeB);

                    Debug.Log(
                        "Merged Mass: " + mergeB.Mass
                    );
                }

                Debug.Log(
                    "World Cluster Count: " +
                    worldClusters.Count
                );

                break;

            default:
                Debug.Log(
                    "All tests complete. Press R to reset."
                );
                return;
        }

        testIndex++;
    }

    void LogCluster(string label, WorldCluster cluster)
    {
        if (cluster == null)
        {
            Debug.Log(label + " | Cluster destroyed/null");
            return;
        }

        Vector2Int visualPosition =
            GridMath.ConvertVector3(
                cluster.transform.position
            );

        Debug.Log(
            label +
            " | Logical: " +
            cluster.logicalWorldPosition +
            " | Visual: " +
            visualPosition +
            " | Mass: " +
            cluster.Mass
        );
    }

    void ResetTests()
    {
        foreach (WorldCluster cluster in worldClusters)
        {
            if (cluster != null)
            {
                Destroy(cluster.gameObject);
            }
        }

        worldClusters.Clear();

        testIndex = 0;

        BuildTests();

        Debug.Log("Tests reset.");
    }
}