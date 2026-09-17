using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClusterCollisionTester : MonoBehaviour
{
    [SerializeField] GameObject pixelPrefab;

    List<WorldCluster> worldClusters = new List<WorldCluster>();

    WorldCluster equalMoving;
    WorldCluster equalBlocking;

    WorldCluster heavyMoving;
    WorldCluster lightBlocking;

    WorldCluster mediumMoving;
    WorldCluster smallBlocking;

    WorldCluster chainA;
    WorldCluster chainB;
    WorldCluster chainC;

    int currentTest = 0;

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
        // TEST 1
        equalMoving = CreateCluster(
            "Equal Moving - Mass 3",
            new Vector2Int(-8, 6),
            3
        );

        equalBlocking = CreateCluster(
            "Equal Blocking - Mass 3",
            new Vector2Int(-4, 6),
            3
        );


        // TEST 2
        heavyMoving = CreateCluster(
            "Heavy Moving - Mass 5",
            new Vector2Int(-8, 1),
            5
        );

        lightBlocking = CreateCluster(
            "Light Blocking - Mass 2",
            new Vector2Int(-4, 1),
            2
        );


        // TEST 3
        mediumMoving = CreateCluster(
            "Medium Moving - Mass 4",
            new Vector2Int(2, 6),
            4
        );

        smallBlocking = CreateCluster(
            "Small Blocking - Mass 2",
            new Vector2Int(6, 6),
            2
        );


        // TEST 4
        chainA = CreateCluster(
            "Chain A - Mass 5",
            new Vector2Int(2, -3),
            5
        );

        chainB = CreateCluster(
            "Chain B - Mass 3",
            new Vector2Int(6, -3),
            3
        );

        chainC = CreateCluster(
            "Chain C - Mass 2",
            new Vector2Int(9, -3),
            2
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
        switch (currentTest)
        {
            case 0:
                Debug.Log(
                    "=== TEST 1 === Mass 3 -> Mass 3 | Strength 6"
                );

                GridPhysics.MoveCluster(
                    equalMoving,
                    Vector2Int.right,
                    6,
                    worldClusters
                );

                break;


            case 1:
                Debug.Log(
                    "=== TEST 2 === Mass 5 -> Mass 2 | Strength 6"
                );

                GridPhysics.MoveCluster(
                    heavyMoving,
                    Vector2Int.right,
                    6,
                    worldClusters
                );

                break;


            case 2:
                Debug.Log(
                    "=== TEST 3 === Mass 4 -> Mass 2 | Strength 5"
                );

                GridPhysics.MoveCluster(
                    mediumMoving,
                    Vector2Int.right,
                    5,
                    worldClusters
                );

                break;


            case 3:
                Debug.Log(
                    "=== TEST 4 === Recursive Chain | 5 -> 3 -> 2 | Strength 8"
                );

                Vector2Int aStart =
                    GridMath.ConvertVector3(chainA.transform.position);

                Vector2Int bStart =
                    GridMath.ConvertVector3(chainB.transform.position);

                Vector2Int cStart =
                    GridMath.ConvertVector3(chainC.transform.position);


                GridPhysics.MoveCluster(
                    chainA,
                    Vector2Int.right,
                    8,
                    worldClusters
                );


                Vector2Int aFinal =
                    GridMath.ConvertVector3(chainA.transform.position);

                Vector2Int bFinal =
                    GridMath.ConvertVector3(chainB.transform.position);

                Vector2Int cFinal =
                    GridMath.ConvertVector3(chainC.transform.position);


                Debug.Log(
                    "CHAIN RESULTS\n" +
                    "A: " + aStart + " -> " + aFinal + "\n" +
                    "B: " + bStart + " -> " + bFinal + "\n" +
                    "C: " + cStart + " -> " + cFinal
                );

                break;


            default:
                Debug.Log(
                    "All collision tests finished. Press R to reset."
                );

                return;
        }

        currentTest++;
    }

    void ResetTests()
    {
        equalMoving.transform.position =
            GridMath.ConvertVector2Int(
                new Vector2Int(-8, 6)
            );

        equalBlocking.transform.position =
            GridMath.ConvertVector2Int(
                new Vector2Int(-4, 6)
            );


        heavyMoving.transform.position =
            GridMath.ConvertVector2Int(
                new Vector2Int(-8, 1)
            );

        lightBlocking.transform.position =
            GridMath.ConvertVector2Int(
                new Vector2Int(-4, 1)
            );


        mediumMoving.transform.position =
            GridMath.ConvertVector2Int(
                new Vector2Int(2, 6)
            );

        smallBlocking.transform.position =
            GridMath.ConvertVector2Int(
                new Vector2Int(6, 6)
            );


        chainA.transform.position =
            GridMath.ConvertVector2Int(
                new Vector2Int(2, -3)
            );

        chainB.transform.position =
            GridMath.ConvertVector2Int(
                new Vector2Int(6, -3)
            );

        chainC.transform.position =
            GridMath.ConvertVector2Int(
                new Vector2Int(9, -3)
            );

        currentTest = 0;

        Debug.Log("Collision tests reset.");
    }
}