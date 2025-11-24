using UnityEngine;

public struct Projection
{
    public float max;
    public float min;
    public Projection(float max, float min)
    {
        this.max = max;
        this.min = min;
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
        lastPosition = transform.position;
        lastRotation = transform.rotation;
        lastScale = transform.lossyScale;
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

    protected  Projection Project(Vector2 axis)
    {
        Vector2[] vertices = GetVertices();

        float min = float.MaxValue;
        float max = float.MinValue; 

        for(int i = 0; i < vertices.Length; i++)
        {
            float p = Vector2.Dot(vertices[i], axis);
            if (p < min) min = p;
            if (p > max) max = p;
        }

        return new Projection(min, max);
    }
}