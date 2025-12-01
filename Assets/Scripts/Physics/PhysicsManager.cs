using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public interface ICollisionEvent
{
    // 물리적 충돌 (밀어내기 발생)
    void OnCustomCollisionEnter(Collider other);
    void OnCustomCollisionStay(Collider other);
    void OnCustomCollisionExit(Collider other);

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
        // Scene에 있는 모든 Collider를 찾아서 리스트에 추가 (성능을 위해 캐싱)
        colliders.AddRange(FindObjectsOfType<Collider>());
    }

    private void FixedUpdate()
    {
        // 이전 프레임의 충돌 목록 초기화
        currnetCollisions.Clear();

        // 1. 모든 쌍에 대해 충돌 검사
        for (int i = 0; i < colliders.Count; i++)
        {
            for (int j = i + 1; j < currnetCollisions.Count; j++)
            {
                Collider a = colliders[i];
                Collider b = colliders[j];

                // 꺼져있는 오브젝트는 무시
                if (!a.gameObject.activeInHierarchy || !b.gameObject.activeInHierarchy) continue;

                // 충돌 검사 실행
                CollisionResult result = CollisionManager.CheckCollision(a, b);

                if (result.isColliding)
                {
                    // 충돌 쌍의 고유 ID 생성
                    string pairID = GetPairId(a, b);
                    currnetCollisions.Add(pairID);

                    // 물리 처리(MTV 적용) - 트리거가 아닌 경우에만
                    if (!a.isTrigger && !b.isTrigger)
                    {
                        ResolvePhysics(a, b, result.mtv);

                        // 이벤트 전송: Collision 물리 충돌
                        HandleCollisionEvents(a, b, pairID, isTriggerEvent: false);
                    }
                    else
                    {
                        // 이벤트 전송: Trigger 통과 충돌
                        HandleCollisionEvents(a, b, pairID, isTriggerEvent: true);
                    }
                }
            }
        }

        // Exit 이벤트 처리 (저번엔 있었는데 이번엔 없는 충돌 쌍들)
        foreach (var pairID in previousCollisions)
        {
            previousCollisions = new HashSet<string>(currnetCollisions);
        }
    }

    private void ResolvePhysics(Collider a, Collider b, Vector2 mtv)
    {
        // 둘 다 움직일 수 있다고 가정하고 반반씩 밀어냄
        a.transform.position -= (Vector3)(mtv * 0.5f);
        b.transform.position += (Vector3)(mtv * 0.5f);
    }

    // 이벤트 분배기 (Enter, Stay 구분) 
    private void HandleCollisionEvents(Collider a, Collider b, string pairId, bool isTriggerEvent)
    {
        bool isStay = previousCollisions.Contains(pairId);

        // A에게 B를 알림
        SendEvent(a, b, isStay, isTriggerEvent);
        // B에게 A를 알림
        SendEvent(b, a, isStay, isTriggerEvent);
    }

    // 실제 인터페이스 호출
    private void SendEvent(Collider me, Collider other, bool isStay, bool isTrigger)
    {
        // 해당 오브젝트에 인터페이스가 구현된 스크립트가 있는지 확인
        var handlers = me.GetComponents<ICollisionEvent>();

        foreach (var handler in handlers)
        {
            if (isTrigger)
            {
                if (isStay) handler.OnCustomTriggerStay(other);
                else        handler.OnCustomTriggerEnter(other);
            }
            else
            {
                if (isStay) handler.OnCustomCollisionStay(other);
                else        handler.OnCustomCollisionEnter(other);
            }
        }
    }


    // A와 B 콜라이더 쌍의 고유 ID 생성
    private string GetPairId(Collider a, Collider b)
    {
        int idA = a.GetInstanceID();
        int idB = b.GetInstanceID();

        return idA < idB ? $"{idA}_{idB}" : $"{idB}_{idA}";
    }
}