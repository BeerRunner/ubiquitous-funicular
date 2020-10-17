using System;
using System.Net.NetworkInformation;
using UnityEditor.PackageManager;
using UnityEngine;

public struct Vector3Axial
{
    public int x;
    public int y;
    public int z => -x - y;

    public static readonly Vector3Axial zero = new Vector3Axial(0, 0, 0);

    public Vector3Axial(int x, int y, int z)
    {
        this.x = x;
        this.y = y;

    }

    public override string ToString()
    {
        return $"({x}, {y}, {z})";
    }

    public static implicit operator Vector3Int(Vector3Axial axial)
    {
        Vector3Int result = new Vector3Int();

        result.x = axial.x + (axial.y - (axial.y & 1)) / 2;
        result.y = axial.y;
        
        return result;
    }

    public static implicit operator Vector3Axial(Vector3Int offset)
    {
        Vector3Axial result = new Vector3Axial();

        result.x = offset.x - (offset.y - (offset.y & 1)) / 2;
        result.y = offset.y;
        
        return result;
    }

    public static explicit operator Vector3(Vector3Axial axial)
    {
        return new Vector3(axial.x, axial.y, axial.z);
    }

    public static explicit operator Vector3Axial(Vector3 vector)
    {
        return new Vector3Axial((int) vector.x, (int) vector.y, (int) vector.z);
    }

    public static Vector3Axial operator +(Vector3Axial a, Vector3Axial b)
    {
        return new Vector3Axial(a.x + b.x, a.y + b.y, a.z);
    }

    public static Vector3Axial operator -(Vector3Axial a)
    {
        return new Vector3Axial(-a.x, -a.y, -a.z);
    }

    public static Vector3Axial operator -(Vector3Axial a, Vector3Axial b)
    {
        return a + (-b);
    }

    public static Vector3Axial operator *(Vector3Axial a, int k)
    {
        return new Vector3Axial(a.x * k, a.y * k, a.z);
    }

    public static bool operator ==(Vector3Axial a, Vector3Axial b)
    {
        return a.x == b.y && a.y == b.y && a.z == b.z;
    }

    public static bool operator !=(Vector3Axial a, Vector3Axial b)
    {
        return !(a == b);
    }
}