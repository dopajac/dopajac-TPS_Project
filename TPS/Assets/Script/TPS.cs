using UnityEngine;

public class TPS : MonoBehaviour
{
    [SerializeField] private Transform characterBody; // 메쉬/애니메이터
    [SerializeField] private Transform cameraArm;     // 카메라 리그(플레이어 기준 포지션)
    
    [Header("Character")]
    [SerializeField] private float moveSpeed = 5f;   // 이동 속도
    [SerializeField] private float turnSpeed = 12f;  // 캐릭터가 이동 방향으로 도는 속도

    [Header("Camera")]
    [SerializeField] private float camYawSpeed   = 220f; // 좌우 회전 속도(도/초 기준 감도)
    [SerializeField] private float camPitchSpeed = 220f; // 상하 회전 속도
    [SerializeField] private float camFollowSpeed = 12f; // 플레이어 따라붙는 속도(위치 보간)
    [SerializeField] private Vector3 camOffset = new Vector3(0, 1.6f, 0f); // 리그 기준 오프셋(선택)

    private Animator animator;
    private float yaw, pitch;

    private void Start()
    {
        if (characterBody) animator = characterBody.GetComponent<Animator>();

        // 초기 각도 저장
        Vector3 e = cameraArm.rotation.eulerAngles;
        yaw = e.y;
        pitch = (e.x > 180f) ? e.x - 360f : e.x;

        // 마우스 잠금(원하면)
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    private void Update()
    {
        LookAround(); // 마우스 입력만 누적
        Move();       // 캐릭터 이동/회전
    }

    private void LateUpdate()
    {
        // 카메라 리그를 플레이어 쪽으로 부드럽게 끌어오기(부모-자식이 아니라면)
        Vector3 targetPos = transform.position + camOffset;
        cameraArm.position = Vector3.Lerp(
            cameraArm.position,
            targetPos,
            1f - Mathf.Exp(-camFollowSpeed * Time.deltaTime)
        );

        // 누적된 yaw/pitch 적용
        cameraArm.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private void Move()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        bool isMove = input.sqrMagnitude > 0.01f;
        if (animator) animator.SetBool("isMove", isMove);
        if (!isMove) return;

        // 카메라 기준 전후/좌우(수평면 투영)
        Vector3 forward = Vector3.ProjectOnPlane(cameraArm.forward, Vector3.up).normalized;
        Vector3 right   = Vector3.ProjectOnPlane(cameraArm.right,   Vector3.up).normalized;
        Vector3 moveDir = (forward * input.y + right * input.x).normalized;

        // 캐릭터를 이동 방향으로 "부드럽게" 회전
        if (moveDir.sqrMagnitude > 0f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            characterBody.rotation = Quaternion.Slerp(characterBody.rotation, targetRot, turnSpeed * Time.deltaTime);
        }

        // 루트 이동
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    private void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw   += mouseX * camYawSpeed   * Time.deltaTime;
        pitch -= mouseY * camPitchSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -70f, 70f);
    }
}
