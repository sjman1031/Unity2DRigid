using UnityEngine;

public struct CollisionResult
{
    public bool isColliding; // 충돌 여부
    public Vector2 mtv;     // Minimum Translation Vector (최소 이동 벡터 -> 방향 * 깊이)
    
    public static CollisionResult NoCollision()
    {
        return new CollisionResult { isColliding = false, mtv = Vector2.zero };
    }

    public static CollisionResult Collided(Vector2 axis, float depth)
    {
        return new CollisionResult { isColliding = true, mtv = axis * depth };
    }
}