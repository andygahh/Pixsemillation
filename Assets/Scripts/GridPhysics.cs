using System.Collections.Generic;
using UnityEngine;

public static class GridPhysics
{
    #region Rotation Physics

    public static int CalculateRotationStrength(Vector2Int bodypixelposition)
    {
        float distance = Mathf.Sqrt(Mathf.Pow(bodypixelposition.x, 2) + Mathf.Pow(bodypixelposition.y, 2));

        return Mathf.RoundToInt(distance);
    }

    public static void Slap(RotationHit hit)
    {
        if (hit != null)
        {
            WorldCluster cluster = hit.struckCluster;

            Vector2Int pushDirection = CalculatePushDirection(hit.strikingPixel);

            int strength = CalculateRotationStrength(hit.strikingPixel.newPosition);

            cluster.transform.position += GridMath.ConvertVector2Int(pushDirection);

            Debug.Log(
                "SLAP | Cluster: " + cluster.name +
                " | Direction: " + pushDirection +
                " | Strength: " + strength +
                " | Position: " + cluster.transform.position
            );
        }
    }

    public static void DebugRotationStrength()
    {
        Debug.Log(CalculateRotationStrength(new Vector2Int(3,4)));
        Debug.Log(CalculateRotationStrength(new Vector2Int(5,0)));
        Debug.Log(CalculateRotationStrength(new Vector2Int(1,1)));
    }

    #endregion

    #region Positioning

    public static bool IsClusterContact(Vector2Int oldPosition, Vector2Int newPosition, Vector2Int ClusterPosition)
    {
        if (newPosition == ClusterPosition)
        {
            return true;
        }

        int steps = Mathf.Max(
            Mathf.Abs(newPosition.x - oldPosition.x),
            Mathf.Abs(newPosition.y - oldPosition.y)
        );

        for (int step = 1; step < steps; step++)
        {
            Vector2 samplePoint = new Vector2();

            float t = step / (float)steps;

            samplePoint.x = Mathf.Lerp(oldPosition.x, newPosition.x, t);
            samplePoint.y = Mathf.Lerp(oldPosition.y, newPosition.y, t);

            Vector2Int sampleCell = Vector2Int.RoundToInt(samplePoint);

            if (sampleCell == ClusterPosition)
            {
                return true;
            }
        }

        return false;
    }

    public static List<MovedPixel> FindMovedPixels(List<Vector2Int> oldPositions,List<Vector2Int> newPositions)
    {
        List<MovedPixel> movedPixels = new List<MovedPixel>();

        for (int i = 0; i < oldPositions.Count; i++)
        {
            Vector2Int oldPosition = oldPositions[i];
            Vector2Int newPosition = newPositions[i];

            if (oldPosition != newPosition)
            {
                MovedPixel pixel = new MovedPixel();

                pixel.oldPosition = oldPositions[i];
                pixel.newPosition = newPositions[i];

                movedPixels.Add(pixel);
            }
        }

        return movedPixels;
    }

    public static MovedPixel FindStrikingPixel(List<Vector2Int> oldBodyPositions, List<Vector2Int> newBodyPositions, Vector2Int playerCore, List<Vector2Int> clusterWorldPositions)
    {
        List<MovedPixel> movedPixels = FindMovedPixels(oldBodyPositions, newBodyPositions);

        foreach (MovedPixel movedPixel in movedPixels)
        {
            Vector2Int oldWorldPosition = playerCore + movedPixel.oldPosition;
            Vector2Int newWorldPosition = playerCore + movedPixel.newPosition;

            foreach (Vector2Int clusterPosition in clusterWorldPositions)
            {
                if (IsClusterContact(oldWorldPosition, newWorldPosition, clusterPosition))
                {
                    return movedPixel;
                }
            }
        }

        return null;
    }

    public static RotationHit FindRotationHit(List<Vector2Int> oldBodyPositions, List<Vector2Int> newBodyPositions, Vector2Int playerCore, List<WorldCluster> worldClusters)
    {
        foreach (WorldCluster cluster in worldClusters)
        {
            if (cluster == null)
            {
                continue;
            }
            
            List<Vector2Int> clusterWorldPositions = cluster.GetWorldPixelPositions();

            MovedPixel strikingPixel = FindStrikingPixel(oldBodyPositions, newBodyPositions, playerCore, clusterWorldPositions);

            if (strikingPixel != null)
            {
                RotationHit hit = new RotationHit();

                hit.strikingPixel = strikingPixel;
                hit.struckCluster = cluster;

                return hit;
            }
            
        }

        return null;
    }

    public static Vector2Int CalculatePushDirection(MovedPixel movedPixel)
    {
        Vector2Int direction = new Vector2Int();
        Vector2Int delta = movedPixel.newPosition - movedPixel.oldPosition;

        direction.x = System.Math.Sign(delta.x);
        direction.y = System.Math.Sign(delta.y);

        return direction;
    }

    public static void DebugClusterContact()
    {
        Debug.Log(IsClusterContact(new Vector2Int(4,0), new Vector2Int(0,-4), new Vector2Int(1,-4)));
        Debug.Log(IsClusterContact(new Vector2Int(0,6), new Vector2Int(4,0), new Vector2Int(4,0)));
    }

    #endregion
}

public class MovedPixel
{
    public Vector2Int oldPosition {get; set;}
    public Vector2Int newPosition {get; set;}
}

public class RotationHit
{
    public MovedPixel strikingPixel {get; set;}
    public WorldCluster struckCluster {get; set;}
}