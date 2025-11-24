using System.Numerics;

public static class CollisionManager
{
    public static bool CheckCollision(Collider a, Collider b)
    {
        // 1. 원이 포함된 경우의 처리
        if (a is CircleCollider || b is CircleCollider)
            return CheckCircleCollision(a, b);
    }

    public static bool CheckSATCollision(Collider a, Collider b)
    {
        return false;
    }

    public static bool CheckAxes(Vector2[] axes, Collider a, Collider b)
    {
        return false;
    }

    public static bool CheckCircleCollision(Collider a, Collider b)
    {
        return false;
    }

    public static Vector2 GetClosestVertex(Vector2[] vertices, Vector2 center)
    {
        return Vector2.Zero;
    }
}