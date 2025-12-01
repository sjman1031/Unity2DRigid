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

    private static CollisionResult CheckSATCollision(Collider a, Collider b)
    {
        float minOverlap = float.MaxValue; // 최소 겹침 길이 
        Vector2 smallestAxis = Vector2.zero; // 그때의 축

        // 1. A의 축 검사
        if(!TryCheckAxes(a.GetAxes(), a, b, ref minOverlap, ref smallestAxis))
            return CollisionResult.NoCollision(); // 분리축 발견 -> 충돌 없음

        // 2. B의 축 검사
        if(!TryCheckAxes(b.GetAxes(), a, b, ref minOverlap, ref smallestAxis))
            return CollisionResult.NoCollision(); // 분리축 발견 -> 충돌 없음

        // 3. 둘 중 하나가 원이라면 동적 축 추가 검사
        // (원의 중심과 가장 가까운 꼭짓점을 잇는 축)
        if(a is CircleCollider || b is CircleCollider)
        {
            CircleCollider circle = (a is CircleCollider) ? (CircleCollider)a : (CircleCollider)b;
            Collider polygon = (a is CircleCollider) ? b : a;

            // 다각형에서 원에 가장 가까운 꼭짓점 찾기
            Vector2 closestVertex = GetClosestVertex(polygon.GetVertices(), circle.WorldCenter);
            Vector2 axis = (circle.WorldCenter - closestVertex).normalized; 

            // 이 축으로 다시 검사
            // 축이 영벡터 -> 중심이 꼭짓점과 겹치면 생략
            if(axis != Vector2.zero)
            {
                if(!TryCheckOneAxis(axis, a, b, ref minOverlap, ref smallestAxis))
                    return CollisionResult.NoCollision(); // 분리축 발견 -> 충돌 없음
            }
        }

        // 4. 방향 보정 (MTV는 항상 A에서 B를 향하는 방향이나 반대로 통일)
        // A -> B 벡터와 MTV 벡터의 내적이 음수면, MTV가 반대라는 뜻이므로 뒤집음
        Vector2 centerDir = b.transform.position - a.transform.position;
        if(Vector2.Dot(centerDir, smallestAxis) < 0)
            smallestAxis = -smallestAxis;

        return CollisionResult.Collided(smallestAxis, minOverlap);
    }

    // 축 목록을 순회하면서 검사
    private static bool TryCheckAxes(Vector2[] axes, Collider a, Collider b, ref float minOverlap, ref Vector2 smallestAxis)
    {
        if (axes == null) return true; // 검사할 축이 없음 = 원 -> 패스

        foreach (var axis in axes)
        {
            if(!TryCheckOneAxis(axis, a, b, ref minOverlap, ref smallestAxis))
                return false; // 분리축 발견 -> 충돌 없음
        }

        return true;
    }

    // 단일 축 검사
    private static bool TryCheckOneAxis(Vector2 axis, Collider a, Collider b, ref float minOverlap, ref Vector2 smallestAxis)
    {
        Projection p1 = a.Project(axis);
        Projection p2 = b.Project(axis);

        float overlap = p1.GetOverlap(p2);

        if (overlap <= 0) return false; // 분리축 발견 -> 충돌 없음

        // 가장 작은 겹침(MTV 후보) 갱신
        if(overlap < minOverlap)
        {
            minOverlap = overlap;
            smallestAxis = axis;
        }

        return true;
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