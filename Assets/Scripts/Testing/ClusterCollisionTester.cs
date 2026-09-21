using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MergeChainTester : MonoBehaviour
{
    [SerializeField] GameObject pixelPrefab;

    List<WorldCluster> worldClusters = new List<WorldCluster>();

    WorldCluster clusterA;
    WorldCluster clusterB;
    WorldCluster clusterC;

    bool testRun = false;

    void Start()
    {
        BuildTest();
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && !testRun)
        {
            RunTest();
            testRun = true;
        }
    }

    void BuildTest()
    {
        /*
         * A: Mass 5, x = 0
         * B: Mass 3, x = 3
         * C: Mass 6, x = 5
         *
         * A is large enough to PUSH B.
         * B is smaller than C, so B should MERGE into C.
         */

        clusterA = CreateCluster(
            "Chain A - Mass 5",
            new Vector2Int(0, 0),
            5
        );

        clusterB = CreateCluster(
            "Chain B - Mass 3",
            new Vector2Int(3, 0),
            3
        );

        clusterC = CreateCluster(
            "Chain C - Mass 6",
            new Vector2Int(5, 0),
            6
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

        // Vertical clusters keep horizontal collision math simple.
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

    void RunTest()
    {
        Debug.Log("=== MERGE CHAIN TEST ===");

        Vector2Int aStart =
            GridMath.ConvertVector3(clusterA.transform.position);

        Vector2Int bStart =
            GridMath.ConvertVector3(clusterB.transform.position);

        Vector2Int cStart =
            GridMath.ConvertVector3(clusterC.transform.position);

        Debug.Log(
            "START | A: " + aStart +
            " | B: " + bStart +
            " | C: " + cStart
        );

        Debug.Log(
            "START MASS | A: " + clusterA.Mass +
            " | B: " + clusterB.Mass +
            " | C: " + clusterC.Mass
        );

        ClusterMoveResult result =
            GridPhysics.MoveCluster(
                clusterA,
                Vector2Int.right,
                6,
                worldClusters
            );

        Vector2Int aFinal =
            GridMath.ConvertVector3(clusterA.transform.position);

        Vector2Int cFinal =
            GridMath.ConvertVector3(clusterC.transform.position);

        Debug.Log(
            "FINAL | A: " + aFinal +
            " | C: " + cFinal
        );

        Debug.Log(
            "FINAL C MASS: " + clusterC.Mass
        );

        Debug.Log(
            "WORLD CLUSTER COUNT: " + worldClusters.Count
        );

        Debug.Log(
            "A RESULT | Vacated: " + result.vacatedSpace +
            " | Remaining Strength: " + result.remainingStrength
        );

        bool aMovedIntoBPosition =
            aFinal == new Vector2Int(3, 0);

        bool mergeMassCorrect =
            clusterC.Mass == 9;

        bool oldClusterRemoved =
            worldClusters.Count == 2;

        Debug.Log(
            "A moved into B's old position: " +
            aMovedIntoBPosition
        );

        Debug.Log(
            "B merged into C correctly: " +
            mergeMassCorrect
        );

        Debug.Log(
            "B removed from worldClusters: " +
            oldClusterRemoved
        );

        if (
            aMovedIntoBPosition &&
            mergeMassCorrect &&
            oldClusterRemoved
        )
        {
            Debug.Log("=== TEST PASSED ===");
        }
        else
        {
            Debug.LogError("=== TEST FAILED ===");
        }
    }
}