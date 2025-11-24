using UnityEngine;

public struct Projection
{
    public float min;
    public float max;

    public Projection(float min, float max)
    {
        this.min = min;
        this.max = max;
    }

    // 투영한 그림자가 겹치는지 여부 반환  
    public bool Overlaps(Projection other)
    {
        // 내 최대값이 상대 최소값보다 작거나, 내 최소값이 상대 최대값보다 작으면 겹치지 않음
        return !(this.max < other.min || other.max < this.min);
    }
}

public abstract class Collider : MonoBehaviour
{
    // 변경 감지용 변수 
    protected Vector3 lastPosition;
    protected Quaternion lastRotation;
    protected Vector3 lastScale;

    protected virtual void Awake()
    {
        // 초기화 시점에 말도 안되는 값으로 설정
        // -> 처음 IsDirty 호출 시 무조건 true 반환
        lastPosition = Vector3.negativeInfinity;
        lastRotation = Quaternion.identity;
        lastScale = Vector3.negativeInfinity;

        // 이후 캐시를 업데이트
        UpdateCache();
    }

    /// <summary>
    /// 다각형의 각 꼭짓점의 좌표를 반환
    /// </summary>
    /// <returns></returns>
    public abstract Vector2[] GetVertices();
    /// <summary>
    /// 각 꼭짓점에 대한 축을 반환
    /// </summary>
    /// <returns></returns>
    public abstract Vector2[] GetAxes();

    protected bool IsDirty()
    {
        if (transform.position   != lastPosition ||
            transform.rotation   != lastRotation ||
            transform.lossyScale != lastScale)
        {
            return true;
        }

        return false;
    }
    
    // 자식들에서 구현
    // last 변수 갱신용
    protected abstract void UpdateCache();

    public  Projection Project(Vector2 axis)
    {
        Vector2[] vertices = GetVertices();

        float min = float.MaxValue;
        float max = float.MinValue; 

        foreach (var v in vertices)
        {
            float p = Vector2.Dot(v, axis);
            if (p < min) min = p;
            if (p > max) max = p;
        }

        return new Projection(min, max);
    }

    protected virtual void OnDrawGizmos()
    {
        // 1. 꼭짓점 가져오기
        Vector2[] vertices = GetVertices();
        if (vertices == null || vertices.Length == 0) return;

        Gizmos.color = Color.green;

        // 2. 선 그리기
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2 current = vertices[i];
            Vector2 next = vertices[(i + 1) % vertices.Length];
            Gizmos.DrawLine(current, next);
        }
    }
}