using UnityEngine;

public class TurretController : MonoBehaviour
{
    [Header("필수 연결")]
    public Transform firePoint;      // 총구
    public GameObject bulletPrefab;  // 총알 프리팹

    [Header("설정")]
    public float range = 30f;        // 사거리 
    [Range(0, 360)]
    public float viewAngle = 60f;    // 시야각 
    public float rotationSpeed = 20f; // 회전 속도 
    public float fireRate = 1f;      // 연사 속도
    public LayerMask targetLayer;    // 적군 레이어

    private Transform target;
    private float fireCountdown = 0f;

    void Update()
    {
        if (target == null)
        {
            // 타겟 없으면: 두리번거리며 찾기
            FindTarget();
            IdleRotate();
        }
        else
        {
            // 타겟 있으면: 묻지도 따지지도 않고 쏜다
            StickyAttack();
        }
    }

    void FindTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 500f, targetLayer);

        foreach (Collider col in colliders)
        {
            Transform potentialTarget = col.transform;

            // 1. 거리 체크 (높이 무시)
            float dist = Vector2.Distance(
                new Vector2(transform.position.x, transform.position.z),
                new Vector2(potentialTarget.position.x, potentialTarget.position.z)
            );

            if (dist <= range)
            {
                // 2. 각도 체크 (발견할 때만 씀)
                Vector3 flatDir = (potentialTarget.position - transform.position).normalized;
                flatDir.y = 0;
                Vector3 flatForward = transform.forward;
                flatForward.y = 0;

                if (Vector3.Angle(flatForward, flatDir) < viewAngle / 2)
                {
                    target = potentialTarget;
                    // ⭐ 발견 즉시 발사 준비
                    fireCountdown = 0f;
                    return;
                }
            }
        }
    }

    // ⭐ 핵심 수정: 각도 계산 삭제! 거리만 되면 무조건 쏨
    void StickyAttack()
    {
        if (target == null) return;

        // 1. 거리 계산 (이탈 조건)
        float dist = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.z),
            new Vector2(target.position.x, target.position.z)
        );

        // 사거리 밖으로 완전히 도망가야만 놔줌
        if (dist > range)
        {
            target = null;
            return;
        }

        // 2. 조준 (Target Look)
        Vector3 dir = target.position - transform.position;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * rotationSpeed).eulerAngles;
        transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0f);

        // 3. 무조건 발사 (각도 체크 안 함)
        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }
        fireCountdown -= Time.deltaTime;
    }

    void IdleRotate()
    {
        transform.Rotate(0, 50 * Time.deltaTime, 0);
    }

    void Shoot()
    {
        if (bulletPrefab && firePoint)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}