using System.Collections.Generic;
using UnityEngine;

public static class GridPhysics
{
    #region Rotation Actions

    public static void Slap(RotationHit hit, List<WorldCluster> worldClusters)
    {
        if (hit != null)
        {
            WorldCluster cluster = hit.struckCluster;

            Vector2Int pushDirection = CalculatePushDirection(hit.strikingPixel);

            int strength = CalculateRotationStrength(hit.strikingPixel.newPosition);

            for (int steps = 0; steps < strength; steps++)
            {
                bool isBlocked = false;

                foreach (WorldCluster worldCluster in worldClusters)
                {
                    if (worldCluster == null || worldCluster.Equals(cluster))
                    {
                        continue;
                    }

                    List<Vector2Int> clusterWorldPixelPositions = worldCluster.GetWorldPixelPositions();

                    foreach (Vector2Int pixel in cluster.GetWorldPixelPositions())
                    {
                        Vector2Int pixelNextPosition = pixel + pushDirection;

                        if (clusterWorldPixelPositions.Contains(pixelNextPosition))
                        {
                            isBlocked = true;
                            break;
                        }
                    }

                    if (isBlocked)
                    {
                        break;
                    }
                }

                if (isBlocked)
                {
                    break;
                }
                else
                {
                    cluster.transform.position += GridMath.ConvertVector2Int(pushDirection);
                }
            }
        }
    }

    #endregion
    
    #region Rotation Calculations
    public static int CalculateRotationStrength(Vector2Int bodypixelposition)
    {
        float distance = Mathf.Sqrt(Mathf.Pow(bodypixelposition.x, 2) + Mathf.Pow(bodypixelposition.y, 2));

        return Mathf.RoundToInt(distance);
    }

    public static Vector2Int CalculatePushDirection(MovedPixel movedPixel)
    {
        Vector2Int direction = new Vector2Int();
        Vector2Int delta = movedPixel.newPosition - movedPixel.oldPosition;

        direction.x = System.Math.Sign(delta.x);
        direction.y = System.Math.Sign(delta.y);

        return direction;
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

    #endregion

    #region Rotation Collision Detection

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

    public static MovedPixel FindStrikingPixel(List<Vector2Int> oldBodyPositions, List<Vector2Int> newBodyPositions, Vector2Int playerCore, List<Vector2Int> clusterWorldPositions)
    {
        List<MovedPixel> movedPixels = FindMovedPixels(oldBodyPositions, newBodyPositions);

        foreach (MovedPixel movedPixel in movedPixels)
        {
            Vector2Int oldWorldPosition = playerCore + movedPixel.oldPosition;
            Vector2Int newWorldPosition = playerCore + movedPixel.newPosition;

            foreach (Vector2Int clusterPosition in clusterWorldPositions)
            {
                if (IsClusterContact(oldWorldPosition, newWorldPosition, clusterPosition, playerCore))
                {
                    return movedPixel;
                }
            }
        }

        return null;
    }

    public static bool IsClusterContact(Vector2Int oldPosition, Vector2Int newPosition, Vector2Int clusterPosition, Vector2Int playerCorePosition)
    {
        Vector2Int oldRelativePosition = oldPosition - playerCorePosition;
        Vector2Int newRelativePosition = newPosition - playerCorePosition;
        Vector2Int expectedClockwise = new Vector2Int(oldRelativePosition.y, -oldRelativePosition.x);

        float radius = Mathf.Sqrt(Mathf.Pow(oldRelativePosition.x, 2) + Mathf.Pow(oldRelativePosition.y, 2));
        float startAngle = Mathf.Atan2(oldRelativePosition.y, oldRelativePosition.x);

        int rotationDirection;

        if (newRelativePosition == expectedClockwise)
        {
            rotationDirection = -1;
        }
        else
        {
            rotationDirection = 1;
        }

        if (newPosition == clusterPosition)
        {
            return true;
        }

        int steps = Mathf.Max(
            Mathf.Abs(newPosition.x - oldPosition.x),
            Mathf.Abs(newPosition.y - oldPosition.y)
        );

        for (int step = 1; step < steps; step++)
        {
            float t = step / (float)steps;
            float angleOffset = (Mathf.PI / 2) * t;
            float sampleAngle = startAngle + rotationDirection * angleOffset;

            Vector2Int sampleCell = GridMath.GetGridPointOnCircle(sampleAngle, radius, playerCorePosition);

            if (sampleCell == clusterPosition)
            {
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Cluster Movement

    public static void MoveCluster(WorldCluster cluster, Vector2Int direction, int strength, List<WorldCluster> worldClusters)
    {
        
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