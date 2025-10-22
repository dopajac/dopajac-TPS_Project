using System;
using UnityEngine;

public class ShootingTargetScore : MonoBehaviour
{
    [Header("과녁 중심 지정")]
    public Transform centerPoint;

    [Header("반경 설정 (중심에서 거리)")]
    private float radiusBullseye = 0.5f;   // 빨간색 (가운데)
    private float radiusYellow   = 0.7f;   // 노란색
    private float radiusOrange   = 0.9f;   // 주황색
    private float radiusBlue     = 1.1f;   // 파란색 (최외곽)

    private void Start()
    {
        // 타겟 크기에 맞게 반경 보정
        radiusBullseye *= transform.localScale.x;
        radiusYellow   *= transform.localScale.x;
        radiusOrange   *= transform.localScale.x;
        radiusBlue     *= transform.localScale.x;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            // 총알 맞은 위치 (Trigger라 Contact 정보 대신 Collider 중심 사용)
            Vector3 hitPoint = other.ClosestPoint(centerPoint.position);
            Vector3 normal = transform.forward;

            // 중심과의 거리 계산
            Vector3 projectedHit = Vector3.ProjectOnPlane(hitPoint - centerPoint.position, normal) + centerPoint.position;
            float distance = Vector3.Distance(projectedHit, centerPoint.position);

            int score = 0;

            if (distance <= radiusBullseye) score = 100;
            else if (distance <= radiusYellow) score = 70;
            else if (distance <= radiusOrange) score = 50;
            else if (distance <= radiusBlue) score = 30;
            else score = 0;

            Debug.Log($"Hit at {distance:F2} → Score: {score}");

            // ✅ 점수 반영
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddTargetScore(score);
                Debug.Log($"Target Score +{score}! 현재 총 점수: {ScoreManager.Instance.Target_Score}");
            }
            else
            {
                Debug.LogWarning("ScoreManager 인스턴스를 찾을 수 없습니다!");
            }

            // 총알 제거
            Destroy(other.gameObject);
            if (gameObject.CompareTag("MoveTarget"))
            {
                return;
            }
            Transform parent = transform.parent;
            if (parent != null)
            {
                parent.gameObject.SetActive(false);
                Debug.Log($"{parent.name} 비활성화됨 (총알 피격)");
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (centerPoint == null) return;

        float scale = transform.localScale.x;

        // Scene 뷰에서 원 표시 (명중 범위 시각화)
        DrawCircle(centerPoint.position, transform.forward, radiusBullseye * scale, Color.red);
        DrawCircle(centerPoint.position, transform.forward, radiusYellow * scale, Color.yellow);
        DrawCircle(centerPoint.position, transform.forward, radiusOrange * scale, new Color(1f, 0.5f, 0f));
        DrawCircle(centerPoint.position, transform.forward, radiusBlue * scale, Color.blue);
    }

    private void DrawCircle(Vector3 center, Vector3 normal, float radius, Color color, int segments = 64)
    {
        Gizmos.color = color;
        Quaternion rotation = Quaternion.LookRotation(normal);
        Vector3 prevPoint = center + rotation * Vector3.right * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = (i * 2 * Mathf.PI) / segments;
            Vector3 nextPoint = center + rotation * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
}
