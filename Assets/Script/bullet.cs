using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 5f; // 3초 뒤 자동 삭제

    void Start()
    {
        Destroy(gameObject, lifeTime); // 생명주기 끝나면 삭제
    }

    void Update()
    {
        // 앞으로만 계속 전진
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    // (선택) 부딪히면 삭제
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 플레이어랑 닿으면
        {
            // 데미지 주는 코드 추가 가능
            Destroy(gameObject);
        }
    }
}