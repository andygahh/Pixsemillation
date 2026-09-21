using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlapTester : MonoBehaviour
{
    [SerializeField] GameObject pixelPrefab;

    private List<WorldCluster> worldClusters = new List<WorldCluster>();

    private WorldCluster singlePixel;
    private WorldCluster singleBlocker;

    private WorldCluster lShape;
    private WorldCluster lShapeBlocker;

    private WorldCluster verticalShape;
    private WorldCluster verticalBlocker;

    private int testIndex = 0;

    void Start()
    {
        BuildTestClusters();
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

    private void BuildTestClusters()
    {
        // TEST 1
        // Single pixel moving right into another single pixel.
        singlePixel = CreateCluster(
            "Single Pixel",
            new Vector2Int(-6, 0),
            new Vector2Int(0, 0)
        );

        singleBlocker = CreateCluster(
            "Single Blocker",
            new Vector2Int(-2, 0),
            new Vector2Int(0, 0)
        );


        // TEST 2
        // L-shaped cluster moving right into a vertical 2-pixel cluster.
        lShape = CreateCluster(
            "L Shape",
            new Vector2Int(0, 3),
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, 1)
        );

        lShapeBlocker = CreateCluster(
            "L Shape Blocker",
            new Vector2Int(4, 3),
            new Vector2Int(0, 0),
            new Vector2Int(0, 1)
        );


        // TEST 3
        // Vertical cluster moving upward into a horizontal cluster.
        verticalShape = CreateCluster(
            "Vertical Shape",
            new Vector2Int(6, -5),
            new Vector2Int(0, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, 2)
        );

        verticalBlocker = CreateCluster(
            "Vertical Blocker",
            new Vector2Int(6, 1),
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0)
        );
    }

    private WorldCluster CreateCluster(
        string clusterName,
        Vector2Int anchor,
        params Vector2Int[] localPixelPositions)
    {
        GameObject root = new GameObject(clusterName);

        root.transform.position = GridMath.ConvertVector2Int(anchor);

        foreach (Vector2Int localPosition in localPixelPositions)
        {
            GameObject pixel = Instantiate(pixelPrefab, root.transform);

            pixel.transform.localPosition =
                GridMath.ConvertVector2Int(localPosition);
        }

        WorldCluster cluster = root.AddComponent<WorldCluster>();

        worldClusters.Add(cluster);

        return cluster;
    }

    private void RunNextTest()
    {
        switch (testIndex)
        {
            case 0:
                Debug.Log("TEST 1: Single pixel -> single blocker");

                TestSlap(
                    singlePixel,
                    Vector2Int.right,
                    6
                );

                break;

            case 1:
                Debug.Log("TEST 2: L shape -> vertical blocker");

                TestSlap(
                    lShape,
                    Vector2Int.right,
                    6
                );

                break;

            case 2:
                Debug.Log("TEST 3: Vertical shape -> horizontal blocker");

                TestSlap(
                    verticalShape,
                    Vector2Int.up,
                    8
                );

                break;

            default:
                Debug.Log("All slap tests complete. Press R to reset.");
                return;
        }

        testIndex++;
    }

    private void TestSlap(
        WorldCluster target,
        Vector2Int direction,
        int strength)
    {
        RotationHit hit = new RotationHit();

        MovedPixel fakeStrikingPixel = new MovedPixel();

        /*
         * GridPhysics.Slap() determines:
         *
         * direction = sign(newPosition - oldPosition)
         * strength = distance of newPosition from (0,0)
         *
         * These fake positions let us directly manufacture
         * a desired cardinal direction and strength.
         */

        fakeStrikingPixel.newPosition = direction * strength;
        fakeStrikingPixel.oldPosition =
            fakeStrikingPixel.newPosition - direction;

        hit.strikingPixel = fakeStrikingPixel;
        hit.struckCluster = target;

        GridPhysics.Slap(hit, worldClusters, new HashSet<Vector2Int>());

        Debug.Log(
            target.name +
            " finished at " +
            target.transform.position
        );
    }

    private void ResetTests()
    {
        singlePixel.transform.position =
            GridMath.ConvertVector2Int(new Vector2Int(-6, 0));

        singleBlocker.transform.position =
            GridMath.ConvertVector2Int(new Vector2Int(-2, 0));

        lShape.transform.position =
            GridMath.ConvertVector2Int(new Vector2Int(0, 3));

        lShapeBlocker.transform.position =
            GridMath.ConvertVector2Int(new Vector2Int(4, 3));

        verticalShape.transform.position =
            GridMath.ConvertVector2Int(new Vector2Int(6, -5));

        verticalBlocker.transform.position =
            GridMath.ConvertVector2Int(new Vector2Int(6, 1));

        testIndex = 0;

        Debug.Log("Slap tests reset.");
    }
}