using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("이동 관련 스탯")]
    public float speed = 5f; // 캐릭터 이동 속도

    private Vector2 inputVec; // 입력 값을 저장할 벡터

    // 컴포넌트를 담을 변수
    private Rigidbody2D rb;
    private SpriteRenderer spriter;

    void Awake()
    {
        // 부모(자신)에게 있는 Rigidbody2D를 가져옵니다.
        rb = GetComponent<Rigidbody2D>();

        // 자식(Visual) 오브젝트에 있는 SpriteRenderer를 가져옵니다.
        spriter = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        // 1. 키보드 입력 받기 (W, A, S, D 또는 방향키)
        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        // 2. 물리 이동 처리
        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + nextVec);
    }

    void LateUpdate()
    {
        // 3. 이동 방향에 따른 좌우 반전(Flip) 처리
        if (inputVec.x != 0)
        {
            spriter.flipX = inputVec.x < 0;
        }
    }
}