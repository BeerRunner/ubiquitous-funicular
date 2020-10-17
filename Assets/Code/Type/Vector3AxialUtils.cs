using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Vector3AxialUtils
{
    public static readonly Vector3Axial[] Directions = new Vector3Axial[6]
    {
        new Vector3Axial(0, 1, 0), // 1 (clockwise direction in hours)
        new Vector3Axial(+1, 0, 0), // 3
        new Vector3Axial(1, -1, 0), // 5
        new Vector3Axial(0, -1, 0), // 7
        new Vector3Axial(-1, 0, 0), // 9
        new Vector3Axial(-1, 1, 0), // 11
    };

    public enum Vector3Direction
    {
        NorthEast = 0,
        East,
        SouthEast,
        SouthWest,
        West,
        NorthWest = 5
    }

    public static Vector3Axial GetNeighbour(this Vector3Axial a, Vector3Direction neighbourDirection)
    {
        return a + Directions[(int) neighbourDirection];
    }

    public static int Distance(this Vector3Axial a, Vector3Axial b)
    {
        return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y), Mathf.Abs(a.z - b.z));
    }

    public static HashSet<Vector3Axial> GetLine(this Vector3Axial a, Vector3Axial b)
    {
        HashSet<Vector3Axial> line = new HashSet<Vector3Axial>();
        int distance = Distance(a, b);
        Vector3Axial direction = b - a;

        if (distance == 0)
        {
            line.Add(a);
            return line;
        }
            
        for (int i = 0; i <= distance; i++)
            line.Add(Lerp(a, b, (1.0f / distance * i)));

        return line;
    }

    public static HashSet<Vector3Axial> GetRing(this Vector3Axial a, int radius)
    {
        HashSet<Vector3Axial> ring = new HashSet<Vector3Axial>();
        Vector3Direction[] directions = Enum.GetValues(typeof(Vector3Direction)).Cast<Vector3Direction>().ToArray();
        Vector3Axial position = a + Directions[4] * radius;

        foreach (Vector3Direction direction in directions)
        {
            for (int j = 0; j < radius; j++)
            {
                ring.Add(position);
                position = GetNeighbour(position, direction);
            }
        }

        return ring;
    }

    public static HashSet<Vector3Axial> GetArea(this Vector3Axial a, int radius)
    {
        HashSet<Vector3Axial> area = new HashSet<Vector3Axial>();

        area.Add(a);

        for (int i = 0; i <= radius; i++)
            area.UnionWith(GetRing(a, i));

        return area;
    }


    public static Vector3Axial Lerp(this Vector3Axial a, Vector3Axial b, float t)
    {
        Vector3 target = (Vector3) a + (Vector3) (b - a) * t;

        int x = (int) Math.Round(target.x, MidpointRounding.AwayFromZero);
        int y = (int) Math.Round(target.y, MidpointRounding.AwayFromZero);
        int z = (int) Math.Round(target.z, MidpointRounding.AwayFromZero);

        float dx = Mathf.Abs(target.x - x);
        float dy = Mathf.Abs(target.y - y);
        float dz = Mathf.Abs(target.z - z);

        if (dx > dy && dx > dz)
            x = -y - z;
        else if (dy > dz)
            y = -x - z;
        else
            z = -x - y;

        if ((x + y + z) != 0)
            Debug.LogError("FUCK UP");

        return new Vector3Axial(x, y, z);
    }
}