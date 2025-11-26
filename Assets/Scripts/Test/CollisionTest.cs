using UnityEngine;

public class  CollisionTest : MonoBehaviour
{
    [Header("테스트할 두 물체 연결")]
    public Collider colliderA;
    public Collider colliderB;

    [Header("렌더러 연결")]
    public MeshRenderer rendererA;
    public MeshRenderer rendererB;

    // 매 프레임 마다 검사
    private void Update()
    {
        if (colliderA == null || colliderB == null) return;
        
        bool isColliding = CollisionManager.CheckCollision(colliderA, colliderB);

        if (isColliding)
        {
            SetColor(Color.red);
        }
        else
        {
            SetColor(Color.white); 
        }
    }

    private void SetColor(Color color)
    {
        if(rendererA != null) rendererA.material.color = color;
        if(rendererB != null) rendererB.material.color = color;
    }
}