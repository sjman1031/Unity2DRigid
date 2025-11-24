using UnityEngine;

public static class CollisionManager
{
    public static bool CheckCollision(Collider a, Collider b)
    {
        // 1. 원이 포함된 경우의 처리
        if (a is CircleCollider || b is CircleCollider)
            return CheckCircleCollision(a, b);

        // 2. 다각형 vs 다각형 (Box vs Box, Box vs Polygon, Polygon vs Polygon)
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

            if(!p1.Overlaps(p2)) return false; // 분리 축 발견
        }

        return true;
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