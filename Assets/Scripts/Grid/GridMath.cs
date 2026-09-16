using System.Collections.Generic;
using UnityEngine;

public static class GridMath
{
    public static Vector2Int ConvertVector3(Vector3 vector)
    {
        Vector2Int vector2 = new Vector2Int();

        vector2.x = Mathf.RoundToInt(vector.x);
        vector2.y = Mathf.RoundToInt(vector.y);

        return vector2;
    }

    public static Vector3 ConvertVector2Int(Vector2Int vector)
    {
        Vector3 vector3 = new Vector3();

        vector3.x = vector.x;
        vector3.y = vector.y;
        vector3.z = 0;

        return vector3;
    }

    public static Vector2Int GetGridPointOnCircle(float angleRadian, float radius, Vector2Int centerPosition)
    {
        Vector2Int newPosition = new Vector2Int();

        newPosition.x = Mathf.RoundToInt(Mathf.Cos(angleRadian) * radius) + centerPosition.x;
        newPosition.y = Mathf.RoundToInt(Mathf.Sin(angleRadian) * radius) + centerPosition.y;

        return newPosition;
    }

    public static List<Vector2Int> Adjacents() 
    {
        return new List<Vector2Int>
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
    }
}