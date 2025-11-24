using UnityEngine;

public class SquareCollider : Collider
{
    public Vector2 size = Vector2.one;
    public Vector2 offset = Vector2.zero;

    // 캐싱된 데이터
    private Vector2[] _cachedVertices  = new Vector2[4];
    private Vector2[] _cachedAxes = new Vector2[2];

    public override Vector2[] GetVertices()
    {
        if (IsDirty()) UpdateCache();
        return _cachedVertices;
    }

    public override Vector2[] GetAxes()
    {
        if (IsDirty()) UpdateCache();
        return _cachedAxes;
    }

    protected override void UpdateCache()
    {
        // 1. 축 업데이트 (회전만 반영)
        _cachedAxes[0] = transform.right;
        _cachedAxes[1] = transform.up;

        // 2. 꼭짓점 업데이트 (위치, 회전, 크기, 오프셋 반영)
        // 로컬 공간에서 4개의 모서리 계산
        float halfW = size.x * 0.5f;
        float halfH = size.y * 0.5f;

        // 로컬 오프셋 적용
        Vector2[] localPos = new Vector2[]
        {
            new Vector2(-halfW + offset.x, -halfH + offset.y), // 좌하단
            new Vector2(-halfW + offset.x,  halfH + offset.y), // 좌상단
            new Vector2( halfW + offset.x, -halfH + offset.y), // 우하단
            new Vector2( halfH + offset.x,  halfH + offset.y)  // 우상단 
        };

        // 월드 좌표로 변환
        for (int i = 0; i < 4; i++)
            _cachedVertices[i] = transform.TransformPoint(localPos[i]);   
        
        // 3. 현재 상태 기록
        lastPosition = transform.position;
        lastRotation = transform.rotation;
        lastScale = transform.lossyScale;
    }
}
