using UnityEngine;

public class PolygonCollider : Collider
{
    public Vector2[] localPoints; // 에디터에서 설정할 점들

    private Vector2[] _cachedVertices;
    private Vector2[] _cachedAxes;

    protected override void Awake()
    {
        if(localPoints != null)
        {
            _cachedVertices = new Vector2[localPoints.Length];
            _cachedAxes = new Vector2[localPoints.Length];  
        }

        // 강제 업데이트
        UpdateCache();
    }

    public override Vector2[] GetVertices()
    {
        if (IsDirty()) UpdateCache();
        return _cachedVertices;
    }

    public override Vector2[] GetAxes()
    {
        if(IsDirty()) UpdateCache();
        return _cachedAxes;
    }

    protected override void UpdateCache()
    {
        if (localPoints == null) return;
        
        // 1. 꼭짓점 반환
        for (int i = 0; i < localPoints.Length; i++)
            _cachedVertices[i] = transform.TransformPoint(localPoints[i]);

        // 2. 축 계산 (법선 벡터)
        for (int i = 0; i < _cachedVertices.Length; i++)
        {
            Vector2 p1 = _cachedVertices[i];
            Vector2 p2 = _cachedVertices[(i + 1) % _cachedVertices.Length];
            Vector2 edge = p2 - p1;

            // 법선 벡터 및 정규화
            _cachedAxes[i] = new Vector2(-edge.y, edge.x).normalized;
        }

        // 3. 상태 기록
        lastPosition = transform.position;
        lastRotation = transform.rotation;
        lastScale = transform.lossyScale;
    }
}
