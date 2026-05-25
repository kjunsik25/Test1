using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10;
    public GameObject bullet;

    private float maxRange = 15f;      // Weapon에서 받아올 최대 사거리
    private float traveledDistance = 0f; // 누적 이동 거리
    private Vector3 lastPosition;        // 이전 프레임의 위치

    private Vector3 targetDirection;
    private bool isHoming = false;
    private float homingForce = 0f;
    private Transform targetEnemy;

    void Start()
    {
        // 시작할 때 최초 위치 기록
        lastPosition = transform.position;
    }

    // Weapon 스크립트에서 사거리를 주입해주는 함수
    public void SetRange(float range)
    {
        maxRange = range;
    }

    // 기본 직선 발사 설정
    public void Launch(Vector3 direction)
    {
        targetDirection = direction.normalized;
        RotateTowardsDirection(targetDirection);
    }

    // 3단계용 고성능 유도 발사 설정
    public void LaunchHoming(Transform target, float force)
    {
        targetEnemy = target;
        isHoming = true;
        homingForce = force;
    }

    void Update()
    {
        // 1. 유도 또는 직선 이동 처리
        if (isHoming && targetEnemy != null && targetEnemy.gameObject.activeSelf)
        {
            // 타겟을 향한 목표 방향 계산
            Vector3 desiredDir = (targetEnemy.position - transform.position).normalized;
            // 현재 방향에서 목표 방향으로 유도력(homingForce)에 따라 부드럽게 회전
            targetDirection = Vector3.Lerp(targetDirection, desiredDir, homingForce * Time.deltaTime).normalized;
        }

        // 계산된 방향으로 실제 이동 및 회전
        RotateTowardsDirection(targetDirection);
        transform.position += targetDirection * speed * Time.deltaTime;

        traveledDistance += Vector3.Distance(lastPosition, transform.position);
        lastPosition = transform.position;

        if (traveledDistance >= maxRange)
        {
            Destroy(gameObject); // 컴포넌트가 아닌 총알 오브젝트 자체를 파괴
        }
    }

    // 총알이 날아가는 방향을 바라보게 회전하는 함수
    void RotateTowardsDirection(Vector3 dir)
    {
        if (dir == Vector3.zero) return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 2. 사거리 체크 (매 프레임 이동 거리를 측정해 누적)
        traveledDistance += Vector3.Distance(lastPosition, transform.position);
        lastPosition = transform.position;

        if (collision.CompareTag("Enemy"))
        {
            // TODO: 적 스크립트를 가져와 데미지를 주는 로직을 여기에 작성하세요.
            // Enemy enemy = collision.GetComponent<Enemy>();
            // if (enemy != null) enemy.TakeDamage(damage);

            Destroy(bullet);
        }
    }
}