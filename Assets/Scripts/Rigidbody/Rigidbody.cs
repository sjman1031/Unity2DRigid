using UnityEngine;

public class Rigidbody : MonoBehaviour
{
    [Header("정적 물리값")]
    public float mass = 1.0f;           // 무게
    public float restitution = 0.5f;    // 반발 계수
    public float friction = 0.1f;       // 마찰 계수
    public bool isStatic = false;       // 벽같은 움직이지 않는 물체인지 판별용

    [Header("동적 물리값")]
    public Vector2 position;
    public Vector2 velocity;
    public Vector2 forceAccumulator; // 한 프레임 동안 가해진 힘의 합

    // 최적화를 위한 질량의 역수 (무한대는 0으로 처리)
    [HideInInspector] public float invMass;

    private void Awake()
    {
        position = transform.position;
        CalculaterMass();
    }

    // 인스펙터에서 값을 바꿀 때마다 재계산
    public void OnValidate()
    {
        CalculaterMass();
    }

    private void CalculaterMass()
    {
        if (isStatic || mass == 0.0f)
        {
            invMass = 0.0f;
            mass    = 0.0f; // 명시적으로 0으로 초기화
        }
        else
        {
            invMass = 1.0f / mass;
        }
    }

    // 외부에서 힘을 가할 때 사용
    public void AddForce(Vector2 force)
    {
        if (isStatic) return;
        forceAccumulator += force;
    }

    // 물리 업데이트
    public void Integrate(float dt)
    {
        if(isStatic) return;
        
        // 1. 가속도 계산 (F = ma => a = F * (1 / m))
        Vector2 acceleration = forceAccumulator * invMass;
        acceleration += Physics2D.gravity; // 유니티 내장 중력 사용

        // 2. 속도 갱신 (v = v0 + at)
        // Semi-Implicit Euler 방식 
        // 속도를 먼저 갱신하고 
        // 그 갱신된 속도를 기반으로 위치를 구함
        velocity += acceleration * dt;

        // 3. 마찰력 적용
        velocity *= (1.0f - friction * dt);

        // 4. 위치 갱신
        position += velocity * dt;

        // 5. 실제 transform에 적용
        transform.position = position;

        // 6. 힘 초기화 
        // 다음 프레임에 이미 적용이 완료된 힘이 남아 있으면 계산에 오류가 생김
        forceAccumulator = Vector2.zero;
    }
}