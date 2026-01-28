using UnityEngine;

public class HelicopterController : MonoBehaviour
{
    [Header("프로펠러 참조 (드래그 앤 드롭)")]
    public Transform Wing;  // 위의 큰 날개
    public Transform Tail_Wing;  // 꼬리 날개

    [Header("속도 설정")]
    public float maxRotorSpeed = 1000f; // 최대 회전 속도
    public float rotorAccel = 500f;     // 회전 가속도 (얼마나 빨리 빨라지는지)
    public float moveSpeed = 5f;        // 상승/하강 속도
    public float turnSpeed = 50f;       // 회전 속도

    // 내부 상태 변수
    private bool isEngineOn = false;    // 시동 켜짐 여부
    private float currentRotorSpeed = 0f; // 현재 프로펠러 속도

    void Update()
    {
        // 1. 시동 제어 (R키)
        if (Input.GetKeyDown(KeyCode.R))
        {
            isEngineOn = !isEngineOn; // 켜져있으면 끄고, 꺼져있으면 킴 (토글)
            Debug.Log(isEngineOn ? "시동 ON" : "시동 OFF");
        }

        // 2. 프로펠러 속도 계산 (서서히 빨라지거나 느려짐)
        if (isEngineOn)
        {
            // 목표 속도(max)까지 가속
            currentRotorSpeed = Mathf.MoveTowards(currentRotorSpeed, maxRotorSpeed, rotorAccel * Time.deltaTime);
        }
        else
        {
            // 0까지 감속
            currentRotorSpeed = Mathf.MoveTowards(currentRotorSpeed, 0f, rotorAccel * Time.deltaTime);
        }

        // 3. 프로펠러 실제로 돌리기 (자식 오브젝트 회전)
        // Y축 기준으로 회전 (모델에 따라 축이 다를 수 있음, 보통 Y)
        if (Wing != null) Wing.Rotate(currentRotorSpeed * Time.deltaTime, 0, 0);
        
        // 꼬리 날개는 보통 X축이나 Z축으로 도는데, 모델에 맞춰 수정 필요 (일단 X로 가정)
        if (Tail_Wing != null) Tail_Wing.Rotate(0, 0, currentRotorSpeed * Time.deltaTime);


        // 4. 움직임 제어 (최대 속도에 도달했을 때만 가능)
        // 약간의 오차를 허용하기 위해 >= maxRotorSpeed * 0.95f 정도로 해도 됨
        if (currentRotorSpeed >= maxRotorSpeed) 
        {
            HandleMovement();
        }
        else if (!isEngineOn && transform.position.y > 0)
        {
            // 시동 끄면 천천히 추락하는 로직
            transform.Translate(Vector3.down * 2f * Time.deltaTime);
        }
    }

    void HandleMovement()
    {
        if (Input.GetKey(KeyCode.W))
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.S))
            transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);

        // 제자리 회전 A(좌) / D(우) - 기존 유지
        if (Input.GetKey(KeyCode.A))
            transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.D))
            transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);

        // [수정] Q = 왼쪽 이동 / E = 오른쪽 이동 (직관적으로 변경)
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Translate(Vector3.left * 5f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Translate(Vector3.right * 5f * Time.deltaTime);
        }
    }
}