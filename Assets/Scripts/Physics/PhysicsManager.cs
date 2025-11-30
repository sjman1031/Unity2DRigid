using UnityEngine;
using System.Collections.Generic;

public interface ICollisionEvent
{
    // 물리적 충돌 (밀어내기 발생)
    void OnCustomCollisionEnter(Collision other);
    void OnCustomCollisionStat(Collision other);
    void OnCustomCollisionExit(Collision other);

    // 트리거 충돌 (통과)
    void OnCustomTriggerEnter(Collider other);
    void OnCustomTriggerStay(Collider other);
    void OnCustomTriggerExit(Collider other);
}

public class PhysicsManager : MonoBehaviour
{
    // Scene에 있는 모든 Collider
    private List<Collider> colliders = new List<Collider>();

    // 지금 충돌중인 쌍을 기억하는 집합
    // HashSet은 중복을 허용하지 않으면서 검색이 빠른 집합이기 때문에 사용
    private HashSet<string> currnetCollisions = new HashSet<string>();
    private HashSet<string> previousCollisions = new HashSet<string>();

    private void Awake()
    {
        colliders.AddRange(FindObjectsOfType<Collider>());
    }

    private void FixedUpdate()
    {
        // 이전 프레임의 충돌 목록 초기화
        currnetCollisions.Clear();

        for (int i = 0; i < colliders.Count; i++)
        {
            for (int j = i + 1; j < currnetCollisions.Count; j++) 
            {
                Collider a = colliders[i];
                Collider b = colliders[j];

                if (!a.gameObject.activeInHierarchy || !b.gameObject.activeInHierarchy) continue;

                CollisionResult result = CollisionManager.CheckCollision(a, b); 
            }
        }
    }

}