using UnityEngine;

public class CircleCollider : Collider
{
    public float radius = 0.5f;
    public Vector2 offset = Vector2.zero;   

    public Vector2 WorldCenter
    {
        get { return (Vector2)transform.position + (Vector2)(transform.rotation * offset); }
    }

    // 원은 고정된 꼭짓점이나 축이 없음
    public override Vector2[] GetVertices() { return null; }
    public override Vector2[] GetAxes() { return null; }

    // 캐싱할게 따로 없어서 구현 생략
    protected override void UpdateCache() { } 

    public new Projection Project(Vector2 axis)
    {
        float centerProjection = Vector2.Dot(WorldCenter, axis);
        // 스케일이 적용된 반지름 계산 (x, y 중 큰 스케일 적용)
        float scaledRadius = radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);

        return new Projection(centerProjection - scaledRadius, centerProjection + scaledRadius);
    }
}
