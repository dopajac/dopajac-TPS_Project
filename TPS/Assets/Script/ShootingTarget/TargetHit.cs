using UnityEngine;

public class TargetHit : MonoBehaviour
{
    [Header("비활성화 딜레이 (즉시 비활성화면 0)")]
    [SerializeField] private float disableDelay = 0f;

    private bool isHit = false;

    private void OnTriggerEnter(Collider other)
    {
        // 총알에 맞았는지 확인
        if (other.CompareTag("Bullet") && !isHit)
        {
            isHit = true; // 중복 방지
            Debug.Log($"{gameObject.name} 피격! 비활성화됩니다.");

            // 총알 제거 (선택)
            Destroy(other.gameObject);

            // 일정 시간 뒤 타겟 비활성화
            if (disableDelay > 0f)
                Invoke(nameof(DisableTarget), disableDelay);
            else
                DisableTarget();
        }
    }

    private void DisableTarget()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        // 다시 활성화될 때 상태 초기화
        isHit = false;
    }
}