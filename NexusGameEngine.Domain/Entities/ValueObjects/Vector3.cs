using System.Reflection;
using NexusGameEngine.Domain.ResultPattern;

namespace NexusGameEngine.Domain.Entities.ValueObjects;

public readonly record struct Vector3
{
    public float X { get; init; }
    public float Y { get; init; }
    public float Z { get; init; }

    private Vector3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static Result<Vector3> Create(float x, float y, float z)
    {
        if (x < 0 || x > 3000) return Error.Validation("Vector3.XNotValid", "The X value is not valid, must be beetwen 0 and 3000 with limits included");
        if (y < 0 || y > 3000) return Error.Validation("Vector3.YNotValid", "The Y value is not valid, must be beetwen 0 and 3000 with limits included");
        if (z < -1000 || z > 1000) return Error.Validation("Vector3.ZNotValid", "The Z value is not valid, must be beetwen -1000 and 1000 with limits included");

        return new Vector3(x, y, z);
    }

    public float CalculateDistance(Vector3 target)
    {
        float dx = target.X - this.X;
        float dy = target.Y - this.Y;
        float dz = target.Z - this.Z;

        return (float)Math.Sqrt((dx * dx) + (dy * dy) + (dz * dz));
    }
}
