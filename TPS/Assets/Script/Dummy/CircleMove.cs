using UnityEngine;

public class CircleMove : MonoBehaviour
{
    public float radius = 5f;       // 원의 반지름
    public float speed = 2f;        // 이동 속도
    private float angle = 0f;       // 각도 값
    private Vector3 center;         // 원의 중심

    void Start()
    {
        // 현재 위치를 원의 중심으로 설정
        center = transform.position;
    }

    void Update()
    {
        // 각도 증가 → 계속 반복됨
        angle += speed * Time.deltaTime;

        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        // 원 궤적 이동
        transform.position = center + new Vector3(x, 0, z);

        // 진행 방향을 바라보도록 회전
        Vector3 dir = new Vector3(-Mathf.Sin(angle), 0, Mathf.Cos(angle));
        transform.rotation = Quaternion.LookRotation(dir);
    }
}
