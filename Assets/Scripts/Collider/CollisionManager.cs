using UnityEngine;

public static class CollisionManager
{
    public static CollisionResult CheckCollision(Collider a, Collider b)
    {
        // 1. 원 - 원 충돌 검사
        if (a is CircleCollider && b is CircleCollider)
            return CheckCircleCircleCollision((CircleCollider)a, (CircleCollider)b);

        // 2. 그 외 모든 경우에는 SAT 이용
        return CheckSATCollision(a, b);
    }

    // 다각형 간 SAT 충돌 검사
    public static bool CheckSATCollision(Collider a, Collider b)
    {
        // A의 축 검사
        if (!CheckAxes(a.GetAxes(), a, b)) return false;

        // B의 축 검사
        if (!CheckAxes(b.GetAxes(), a, b)) return false;
        
        return true; // 모든 축에서 겹침 
    }

    public static bool CheckAxes(Vector2[] axes, Collider a, Collider b)
    {
        foreach (var axis in axes)
        {
            Projection p1 = a.Project(axis);
            Projection p2 = b.Project(axis);

            if(!p1.Overlaps(p2))
            {
                Debug.Log($"분리 축 발견! Axis: {axis}, P1: {p1.min}~{p1.max}, P2: {p2.min}~{p2.max}");
                return false; // 분리 축 발견
            }
        }

        return true;
    }

    private static CollisionResult CheckCircleCircleCollision(CircleCollider a, CircleCollider b)
    {
        Vector2 dir = a.WorldCenter - b.WorldCenter;
        float distSq = dir.sqrMagnitude; // 거리의 제곱 (성능 최적화 용)

        // 스케일을 고려한 반지름 계산
        float radiusA = a.radius * Mathf.Max(a.transform.lossyScale.x, a.transform.lossyScale.y);
        float radiusB = b.radius * Mathf.Max(b.transform.lossyScale.x, b.transform.lossyScale.y);
        float radiusSum = radiusA + radiusB;

        // 충돌 안함
        if (distSq > radiusSum * radiusSum)
        {
            return CollisionResult.NoCollision(); // 충돌 없음
        }

        // 충돌 계산 -> MTV 계산
        float dist = Mathf.Sqrt(distSq);

        // 거리가 0이면 임의의 방향으로 밀어냄
        if (dist == 0f) dist = 0.001f;

        float overlap = radiusSum - dist;
        Vector2 normal = dir / dist; // B -> A 방향의 단위 벡터

        return CollisionResult.Collided(normal, overlap);
    }

    public static bool CheckCircleCollision(Collider a, Collider b)
    {
        // 둘 다 원인 경우
        if (a is CircleCollider c1 && b is CircleCollider c2)
        {
            float distSq = (c1.WorldCenter - c2.WorldCenter).sqrMagnitude;

            // x와 y의 스케일 중 더 큰값을 반지름에 적용
            float r1 = c1.radius * Mathf.Max(c1.transform.lossyScale.x, c1.transform.lossyScale.y);
            float r2 = c2.radius * Mathf.Max(c2.transform.lossyScale.x, c2.transform.lossyScale.y);

            float radiusSum = r1 + r2;
            return distSq < radiusSum * radiusSum;
        }

        // 하나만 원 인 경우
        CircleCollider circle = (a is CircleCollider) ? (CircleCollider)a : (CircleCollider)b;
        Collider polygon = (a is CircleCollider) ? b : a;

        // 1. 다각형의 축 검사
        if (!CheckAxes(polygon.GetAxes(), polygon, circle)) return false;

        // 2. 원 중심 -> 가장 가까운 꼭짓점 축 검사
        Vector2 closest = GetClosestVertex(polygon.GetVertices(), circle.WorldCenter);
        Vector2 axis = (closest - circle.WorldCenter).normalized;

        Projection pPolygon = polygon.Project(axis);
        Projection pCircle = circle.Project(axis);

        return pPolygon.Overlaps(pCircle);
    }

    public static Vector2 GetClosestVertex(Vector2[] vertices, Vector2 center)
    {
        Vector2 closest = Vector2.zero;
        float minDist = float.MaxValue;

        foreach (var vertex in vertices)
        {
            float dist = (vertex - center).sqrMagnitude;
            if (dist < minDist)
            {
                minDist = dist;
                closest = vertex;
            }
        }

        return closest;
    }
}