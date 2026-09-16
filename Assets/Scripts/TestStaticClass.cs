using System.Collections.Generic;
using UnityEngine;

public class TestStaticClass : MonoBehaviour
{
    void Start()
    {
        List<Vector2Int> oldBodyPositions = new List<Vector2Int>()
        {
            new Vector2Int(0, 0),
            new Vector2Int(4, 0)
        };

        List<Vector2Int> newBodyPositions = new List<Vector2Int>()
        {
            new Vector2Int(0, 0),
            new Vector2Int(0, -4)
        };

        Vector2Int playerCore = new Vector2Int(10, 10);

        List<Vector2Int> clusterWorldPositions = new List<Vector2Int>()
        {
            new Vector2Int(12, 8)
        };

        MovedPixel strikingPixel = GridPhysics.FindStrikingPixel(
            oldBodyPositions,
            newBodyPositions,
            playerCore,
            clusterWorldPositions
        );

        if (strikingPixel != null)
        {
            Debug.Log(
                "SWEEP HIT DETECTED | Old: " +
                strikingPixel.oldPosition +
                " | New: " +
                strikingPixel.newPosition +
                " | Strength: " +
                GridPhysics.CalculateRotationStrength(strikingPixel.newPosition)
            );
        }
        else
        {
            Debug.Log("No sweep hit detected.");
        }
    }
}