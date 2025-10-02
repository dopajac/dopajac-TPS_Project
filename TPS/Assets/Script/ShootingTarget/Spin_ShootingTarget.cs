using System.Collections;
using UnityEngine;

public class Spin_ShootingTarget : MonoBehaviour
{
    [SerializeField] private float flipSpeed = 2f; // 회전 속도
    private bool isRed = true;                     // 현재 빨강 상태?
    private bool isFlipping = false;

    private Quaternion redRotation;    // 초기 (빨강 앞면)
    private Quaternion greenRotation;  // 반대 (초록 앞면)

    void Start()
    {
        // Branch의 초기 회전 저장
        redRotation = transform.rotation;
        // Body의 y축을 기준으로 180도 뒤집기
        greenRotation = redRotation * Quaternion.Euler(0, 180, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet") && !isFlipping)
        {
            // 맞으면 현재 상태 반대로 토글
            if (isRed)
                StartCoroutine(FlipRoutine(redRotation, greenRotation, false));
            else
                StartCoroutine(FlipRoutine(greenRotation, redRotation, true));

            Destroy(collision.gameObject);
        }
    }

    private IEnumerator FlipRoutine(Quaternion from, Quaternion to, bool nextIsRed)
    {
        isFlipping = true;
        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * flipSpeed;
            transform.rotation = Quaternion.Slerp(from, to, t);
            yield return null;
        }

        isRed = nextIsRed;
        isFlipping = false;
    }
}
