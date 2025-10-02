using System;
using UnityEngine;

public class ShootingTargetScore : MonoBehaviour
{
    [Header("과녁 중심 지정")]
    public Transform centerPoint;

    [Header("반경 설정 (중심에서 거리)")]
    private float radiusBullseye = 0.12f;   // 빨간색 (가운데)
    private float radiusYellow   = 0.17f;   // 노란색
    private float radiusOrange   = 0.23f;   // 주황색
    private float radiusBlue     = 0.33f;   // 파란색 (최외곽)

    private void Start()
    {
        radiusBullseye = radiusBullseye * transform.localScale.x;
        radiusYellow  = radiusYellow * transform.localScale.x;
        radiusOrange  = radiusOrange * transform.localScale.x;
        radiusBlue  = radiusBlue * transform.localScale.x;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // 총알 맞은 위치
            Vector3 hitPoint = collision.contacts[0].point;
            Vector3 normal = transform.forward;
            // 중심과의 거리
            Vector3 projectedHit = Vector3.ProjectOnPlane(hitPoint - centerPoint.position, normal) 
                                   + centerPoint.position;
            float distance = Vector3.Distance(projectedHit, centerPoint.position);
            
            int score = 0;

            if (distance <= radiusBullseye) score = 100;   // 한가운데
            else if (distance <= radiusYellow) score = 70;
            else if (distance <= radiusOrange) score = 50;
            else if (distance <= radiusBlue) score = 30;
            else score = 0; // 빗나감

            Debug.Log($"Hit at {distance:F2} → Score: {score}");

            // 총알 제거
            Destroy(collision.gameObject);
        }
    }
    private void OnDrawGizmos()
    {
        if (centerPoint == null) return;

        // Scene 뷰에서 "원"을 표시하기 위해 평면 방향으로 그리기
        DrawCircle(centerPoint.position, transform.forward, radiusBullseye, Color.red);
        DrawCircle(centerPoint.position, transform.forward, radiusYellow, Color.yellow);
        DrawCircle(centerPoint.position, transform.forward, radiusOrange, new Color(1f, 0.5f, 0f));
        DrawCircle(centerPoint.position, transform.forward, radiusBlue, Color.blue);
    }

    // 평면 위 원 그리기
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