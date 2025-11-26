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

    public override Projection Project(Vector2 axis)
    {
        Vector2 worldCenter = (Vector2)transform.position + (Vector2)(transform.rotation * offset);

        float centerProjection = Vector2.Dot(worldCenter, axis);

        float currentScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
        float scaledRadius = radius * currentScale;

        return new Projection(centerProjection - scaledRadius, centerProjection + scaledRadius);
    }


    protected override void OnDrawGizmos()
    {
        // 부모의 기능은 무시하고 재정의
        
        
        Gizmos.color = Color.green;

        // 1. 월드 좌표계에서 중심 계산
        // transform.position + (회전 * 오프셋)
        Vector2 worldCenter = (Vector2)transform.position + (Vector2)(transform.rotation * offset);

        // 2. 실제 월드상의 반지름 계산 (가장 긴 축을 기준)
        float currentScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y) * radius;
        float finalRadius = currentScale * radius;

        // 3. 원 그리기
        Gizmos.DrawWireSphere(worldCenter, finalRadius);
    }
}
