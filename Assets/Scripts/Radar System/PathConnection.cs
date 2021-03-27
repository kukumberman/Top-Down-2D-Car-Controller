using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathConnection
{
    public Vector3 A { get; }
    public Vector3 B { get; }

    public PathConnection(Vector3 a, Vector3 b)
    {
        A = a;
        B = b;
    }

    public bool IsEqual(PathConnection connection)
    {
        return (A == connection.A && B == connection.B) || (A == connection.B && B == connection.A);
    }

    public Vector3 Project(Vector3 position, out bool isInside, out float t)
    {
        Vector3 ac = position - A;
        Vector3 ab = B - A;

        Vector3 result = A + Vector3.Project(ac, ab);

        isInside = IsInside(result, out t);

        return result;
    }

    private bool IsInside(Vector3 projectedPosition, out float t)
    {
        t = InverseLerp(A, B, projectedPosition);

        return t >= 0 && t <= 1;
    }

    private float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
    {
        Vector3 AB = b - a;
        Vector3 AV = value - a;
        return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
    }
}
