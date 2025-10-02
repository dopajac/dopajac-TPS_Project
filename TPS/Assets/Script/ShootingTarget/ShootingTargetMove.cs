using UnityEngine;

public class ShootingTargetMove : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform controlPoint; // 곡선 모양을 정하는 중간점
    [SerializeField] private Transform endPoint;
    [SerializeField] private float moveSpeed = 1f;

    private float t = 0f;
    private bool forward = true;

    void Update()
    {
        t += (forward ? 1 : -1) * moveSpeed * Time.deltaTime;

        if (t >= 1f)
        {
            t = 1f;
            forward = false;
        }
        else if (t <= 0f)
        {
            t = 0f;
            forward = true;
        }

        // 2차 베지어 곡선 공식
        Vector3 a = Vector3.Lerp(startPoint.position, controlPoint.position, t);
        Vector3 b = Vector3.Lerp(controlPoint.position, endPoint.position, t);
        transform.position = Vector3.Lerp(a, b, t);
    }
}
