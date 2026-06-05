using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("무기 정보 및 프리펩")]
    public GameObject bulletPrefab;
    [Range(1, 3)] public int weaponStage = 1; // 1, 2, 3단계 조절 변수
    [Range(1, 3)] public int weaponLevel = 1; // 1, 2, 3단계의 무기 레벨
    public float scanRange = 10f;
    public float weaponRange = 15f; // 총알에 전달할 최대 사거리

    [Header("1단계 설정 (재장전)")]
    public int maxAmmo = 10;
    private int currentAmmo = 10;
    public float reloadDuration = 2f;
    private bool isReloading = false;

    // 타이머 변수들
    private float fireTimer;
    private float burstTimer; // 3단계 전방위 발사용
    private Transform target;

    void Start()
    {
        currentAmmo = maxAmmo;
    }

    void Update()
    {
        // 1단계 재장전 중이면 공격 메커니즘 일시 정지
        if (weaponStage == 1 && isReloading) return;

        fireTimer += Time.deltaTime;
        if (weaponStage == 3) burstTimer += Time.deltaTime;

        // --- 기본 자동 공격 타이머 체크 ---
        float currentFireRate = (weaponStage == 3) ? 0.25f : 0.5f; // 3단계는 0.25초, 나머지는 0.5초

        if (fireTimer >= currentFireRate)
        {
            fireTimer = 0f;
            FindClosestEnemy();

            if (target != null)
            {
                FirePattern();
            }
        }

        // --- 3단계 전방위 추가 공격 타이머 체크 ---
        if (weaponStage == 3 && burstTimer >=5f)
        {
            switch (weaponLevel)
            {
                case 1:

                    burstTimer = 0f;
                    FindClosestEnemy();
                    FireOmniDirectional();

                    break;

                case 2:

                    burstTimer = 2.5f;
                    FindClosestEnemy();
                    FireOmniDirectional();

                    break;

                case 3:

                    burstTimer = 4f;
                    FindClosestEnemy();
                    FireOmniDirectional();

                    break;
            }
            
        }
    }

    // 각 단계별 발사 분기 처리
    void FirePattern()
    {
        Vector3 baseDir = (target.position - transform.position).normalized;

        switch (weaponStage)
        {
            case 1:

                // 1단계: 1발씩 발사 후 잔탄수 감소
                switch (weaponLevel)
                {
                    case 1:
                        CreateBullet(baseDir, false);

                        currentAmmo--;

                    
                        break;
                    case 2:
                        GameObject upBullet = Instantiate(bulletPrefab, transform.position + Vector3.up * 0.4f, transform.rotation);
                        GameObject downBullet = Instantiate(bulletPrefab, transform.position + Vector3.down * 0.4f, transform.rotation);

                        Vector3 upDir = new Vector3 (baseDir.x, baseDir.y ,baseDir.z );
                        Vector3 downDir = new Vector3(baseDir.x, baseDir.y, baseDir.z);

                        Bullet bullet = upBullet.GetComponent<Bullet>();
                        if (bullet != null)
                        {
                            bullet.SetRange(weaponRange);

                            bullet.Launch(upDir);
                        }
                        Bullet bullet2 = downBullet.GetComponent<Bullet>();
                        if (bullet2 != null)
                        {
                            bullet2.SetRange(weaponRange);

                            bullet2.Launch(downDir);
                        }
                        currentAmmo--;

                        break;
                    case 3:

                        GameObject upBullet_1 = Instantiate(bulletPrefab, transform.position + Vector3.up *0.4f, transform.rotation);
                        GameObject upBullet_2 = Instantiate(bulletPrefab, transform.position + Vector3.up * 0.8f, transform.rotation);
                        GameObject downBullet_1 = Instantiate(bulletPrefab, transform.position + Vector3.down * 0.4f, transform.rotation);
                        GameObject downBullet_2 = Instantiate(bulletPrefab, transform.position + Vector3.down * 0.8f, transform.rotation);


                        Vector3 upDir_1 = new Vector3(baseDir.x,baseDir.y,baseDir.z);

                        Bullet upbullet1 = upBullet_1.GetComponent<Bullet>();
                        if (upbullet1 != null)
                        {
                            upbullet1.SetRange (weaponRange);
                            upbullet1.Launch(upDir_1);
                        }

                        Bullet upbullet2 = upBullet_2.GetComponent<Bullet>();
                        if (upbullet2 != null)
                        {
                            upbullet2.SetRange(weaponRange);
                            upbullet2.Launch(upDir_1);
                        }

                        Bullet downbullet1 = downBullet_1.GetComponent<Bullet>();
                        if (downbullet1 != null)
                        {
                            downbullet1.SetRange(weaponRange);
                            downbullet1.Launch(upDir_1);
                        }

                        Bullet downbullet2 = downBullet_2.GetComponent<Bullet>();
                        if (downbullet2 != null)
                        {
                            downbullet2.SetRange(weaponRange);
                            downbullet2.Launch(upDir_1);
                        }

                        CreateBullet(baseDir, false);

                        currentAmmo--;
                        
                        break;
                }

                if (currentAmmo <= 0)
                {
                    StartCoroutine(ReloadRoutine());
                }
                break;

            case 2:
                // 2단계: 메인(적 방향) 1발 + 120도 간격 사이드 2발 (삼각형 배치)
                CreateBullet(baseDir, false);

                switch (weaponLevel)
                {



                    // 좌우 60도 회전된 방향 계산
                    case 1: //3발 샷건
                        Vector3 leftDir = Quaternion.Euler(0, 0, 60) * baseDir;
                        Vector3 rightDir = Quaternion.Euler(0, 0, -60) * baseDir;

                        CreateBullet(leftDir, false);  // 뒤쪽 대각선 탄 1 (유도 없음)
                        CreateBullet(rightDir, false); // 뒤쪽 대각선 탄 2 (유도 없음)
                        break;
                    case 2: // 5발 샷건
                        Vector3 leftDir1 = Quaternion.Euler(0, 0, 60) * baseDir;
                        Vector3 leftDir2 = Quaternion.Euler(0, 0, 30) * baseDir;

                        Vector3 rightDir1 = Quaternion.Euler(0, 0, -60) * baseDir;
                        Vector3 rightDir2 = Quaternion.Euler(0, 0, -30) * baseDir;

                        CreateBullet(leftDir1, false);  // 뒤쪽 대각선 탄 1 (유도 없음)
                        CreateBullet(rightDir1, false); // 뒤쪽 대각선 탄 2 (유도 없음)
                        CreateBullet(leftDir2, false);  // 뒤쪽 대각선 탄 1 (유도 없음)
                        CreateBullet(rightDir2, false); // 뒤쪽 대각선 탄 2 (유도 없음)
                        break;
                    case 3: // 9발 샷건
                        Vector3 leftDir3_1 = Quaternion.Euler(0, 0, 15) * baseDir;
                        Vector3 leftDir3_2 = Quaternion.Euler(0, 0, 30) * baseDir;
                        Vector3 leftDir3_3 = Quaternion.Euler(0, 0, 45) * baseDir;
                        Vector3 leftDir3_4 = Quaternion.Euler(0, 0, 60) * baseDir;


                        Vector3 rightDir3_1 = Quaternion.Euler(0, 0, -15) * baseDir;
                        Vector3 rightDir3_2 = Quaternion.Euler(0, 0, -30) * baseDir;
                        Vector3 rightDir3_3 = Quaternion.Euler(0, 0, -45) * baseDir;
                        Vector3 rightDir3_4 = Quaternion.Euler(0, 0, -60) * baseDir;

                        CreateBullet(leftDir3_1, false);  // 뒤쪽 대각선 탄 1 (유도 없음)
                        CreateBullet(rightDir3_1, false); // 뒤쪽 대각선 탄 2 (유도 없음)
                        CreateBullet(leftDir3_2, false);  // 뒤쪽 대각선 탄 1 (유도 없음)
                        CreateBullet(rightDir3_2, false); // 뒤쪽 대각선 탄 2 (유도 없음)
                        CreateBullet(leftDir3_3, false);
                        CreateBullet(rightDir3_3, false);
                        CreateBullet(leftDir3_4, false);
                        CreateBullet(rightDir3_4, false);

                        break;

                }
                break;
            case 3:
                
                

                // 3단계: 기본 0.25초 주기 탄 (고성능 유도)
                CreateBullet(baseDir, true);
                break;
        }
    }

    // 3단계 전방위 추가 패턴 (1초마다 8방향)
    void FireOmniDirectional()
    {
        for (int i = 0; i < 8; i++)
        {
            // 360도를 8로 나누어 45도 간격으로 발사 방향 수립
            float angle = i * 45f;
            Vector3 dir = Quaternion.Euler(0, 0, angle) * Vector3.right;

            // 어느 방향으로 생성되든 강한 유도력을 지니고 타겟을 쫓아감
            CreateBullet(dir, true);
        }
    }

    // 총알 생성 공용 헬퍼 함수
    void CreateBullet(Vector3 direction, bool useStrongHoming)
    {
        if (bulletPrefab == null) return;

        GameObject bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Bullet bullet = bulletObj.GetComponent<Bullet>();

        if (bullet != null)
        {
            // [중요] 사거리를 Bullet 스크립트에 넘겨주어 자체적으로 파괴되도록 유도합니다.
            bullet.SetRange(weaponRange);

            if (useStrongHoming && target != null)
            {
                bullet.Launch(direction);
                
                bullet.LaunchHoming(target, 15f);

            }
            else
            {
                bullet.Launch(direction);
            }
        }
    }

    // 1단계용 재장전 코루틴
    System.Collections.IEnumerator ReloadRoutine()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadDuration);
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    // 가장 가까운 적 탐색 로직
    void FindClosestEnemy()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, scanRange);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (Collider2D t in targets)
        {
            if (t.CompareTag("Enemy"))
            {
                float distanceToEnemy = Vector3.Distance(transform.position, t.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = t.transform;
                }
            }
        }
        target = closestEnemy;
    }

    // 에디터 뷰에서 스캔 범위를 시각적으로 확인하기 위한 기즈모
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, scanRange);
    }
}