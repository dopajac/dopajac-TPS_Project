using UnityEngine;

public class Spin180 : MonoBehaviour
{
    [Header("회전 속도 설정")]
    [SerializeField] private float rotateSpeed = 180f;

    private bool isRotating = false;
    private Quaternion targetRotation;

    private void Update()
    {
        if (isRotating)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

            // 목표 각도에 거의 도달했으면 멈추기
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                transform.rotation = targetRotation;
                isRotating = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Bullet")) return; // 총알만 감지

        // 총알이 어디 방향에서 들어왔는지 계산
        Vector3 hitDirection = (other.transform.position - transform.position).normalized;

        // 타겟의 앞 방향(로컬 z축)
        float dot = Vector3.Dot(transform.forward, hitDirection);

        // dot이 양수면 정면에서 맞은 것, 음수면 뒤에서 맞은 것
        if (dot > 0)
            RotateToBack(); // 앞에서 맞음 → 뒤로 회전
        else
            RotateToFront(); // 뒤에서 맞음 → 앞으로 회전

        // 총알 제거
        Destroy(other.gameObject);
    }

    private void RotateToBack()
    {
        if (isRotating) return;
        targetRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, 180f, 0f));
        isRotating = true;
    }

    private void RotateToFront()
    {
        if (isRotating) return;
        targetRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, -180f, 0f));
        isRotating = true;
    }
}