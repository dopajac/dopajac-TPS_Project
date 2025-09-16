using System;
using StarterAssets;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerManager : MonoBehaviour
{
    private StarterAssetsInputs input;
    private ThirdPersonController controller;
    private Animator anim;

    [Header("Aim")]
    [SerializeField] private CinemachineVirtualCamera aimCam;
    [SerializeField] private GameObject aimImage;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private GameObject aimObj;
    [SerializeField] private float aimObjDis = 50f;
    [SerializeField] private Cinemachine3rdPersonFollow aimCam_cm;

    [Header("Zoom (Tap)")]
    [SerializeField] private float zoom_cm_Distance = -5f;
    private float base_cm_Distance;
    private bool zoomOn;

    [Header("IK")]
    [SerializeField] private Rig handRig;
    [SerializeField] private Rig aimRig;

    [Header("Charged Jump")]
    [SerializeField] private float minJumpHeight = 0.8f;
    [SerializeField] private float maxJumpHeight = 9.0f;
    [SerializeField] private AnimationCurve chargeToHeight = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Camera Bullet Origin")]
    [SerializeField] private float cameraSpawnOffset = 1f; // 카메라 앞쪽으로 조금 이동해 시작
    [SerializeField] private LayerMask obstacleMask = ~0;     // 배럴체크에 사용할 레이어(선택)

    private void Start()
    {
        input = GetComponent<StarterAssetsInputs>();
        controller = GetComponent<ThirdPersonController>();
        if (aimCam_cm == null && aimCam != null)
            aimCam_cm = aimCam.GetComponent<Cinemachine3rdPersonFollow>();

        if (aimCam_cm != null)
            base_cm_Distance = aimCam_cm.CameraDistance;

        anim = GetComponent<Animator>();
        ApplyZoom();
    }

    private void Update()
    {
        // 줌 토글
        if (input.ConsumeZoomTap())
        {
            zoomOn = !zoomOn;
            ApplyZoom();
        }

        // 조준/사격 처리
        AimCheck();

        // 차징 점프
        if (input.ConsumeChargeReleased(out float chargeSec))
        {
            float t = (input.chargeMax > 0f) ? Mathf.Clamp01(chargeSec / input.chargeMax) : 0f;
            float h01 = Mathf.Clamp01(chargeToHeight.Evaluate(t));
            float jumpHeight = Mathf.Lerp(minJumpHeight, maxJumpHeight, h01);
            controller.ChargedJump(jumpHeight);
        }
    }

    private void ApplyZoom()
    {
        if (aimCam_cm == null) return;
        aimCam_cm.CameraDistance = zoomOn ? zoom_cm_Distance : base_cm_Distance;
    }

    private void AimCheck()
    {
        // 리로드 처리
        if (input.reload)
        {
            input.reload = false;
            if (!controller.isReLoad)
            {
                AimControll(false);
                SetRigWeight(0);
                anim.SetLayerWeight(1, 1);
                anim.SetTrigger("Reload");
                controller.isReLoad = true;
            }
        }
        if (controller.isReLoad) return;

        Vector3 targetPosition = GetShootTarget();

        if (input.adsHolding)
        {
            // 조준 상태
            AimControll(true);
            anim.SetLayerWeight(1, 1);
            SetRigWeight(1);

            // 캐릭터를 타겟 방향으로 회전
            Vector3 targetFlat = targetPosition; 
            targetFlat.y = transform.position.y;
            Vector3 aimDir = (targetFlat - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * 50f);

            if (input.shoot && CanShootThisFrame())
            {
                // ADS 시: 카메라 중앙(origin)에서 발사
                Vector3 camOrigin = GetCameraSpawnOrigin();
                bool useCameraOrigin = CanUseCameraOrigin(camOrigin);
                GameManager.instance.Shooting(targetPosition, useCameraOrigin, camOrigin);

                anim.SetBool("Shoot", true);
            }
            else anim.SetBool("Shoot", false);
        }
        else
        {
            // 비조준 상태
            AimControll(false);
            SetRigWeight(0);
            anim.SetLayerWeight(1, 0);

            if (input.shoot && CanShootThisFrame())
            {
                // 줌만 켜져 있으면 카메라에서, 아니면 총구에서 (원하는 정책으로 바꿔도 됨)
                bool fireFromCamera = zoomOn;
                Vector3 camOrigin = GetCameraSpawnOrigin();
                if (fireFromCamera)
                {
                    bool useCameraOrigin = CanUseCameraOrigin(camOrigin);
                    GameManager.instance.Shooting(targetPosition, useCameraOrigin, camOrigin);
                }
                else
                {
                    GameManager.instance.Shooting(targetPosition);
                }

                // 약간 몸 방향 보정
                Vector3 targetFlat = targetPosition; 
                targetFlat.y = transform.position.y;
                Vector3 aimDir = (targetFlat - transform.position).normalized;
                transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * 15f);

                anim.SetBool("Shoot", true);
            }
            else anim.SetBool("Shoot", false);
        }
    }

    private void AimControll(bool isCheck)
    {
        if (aimCam)  aimCam.gameObject.SetActive(isCheck);
        if (aimImage) aimImage.SetActive(isCheck);
        controller.isAimMove = isCheck;
    }

    public void Reroad() // 애니메이션 이벤트에서 호출
    {
        controller.isReLoad = false;
        SetRigWeight(1);
        anim.SetLayerWeight(1, 0);
    }

    public void SetRigWeight(float weight)
    {
        if (aimRig)  aimRig.weight = weight;
        if (handRig) handRig.weight = weight; 
    }

    public void ReLoadWeaponClip() // 애니메이션 이벤트에서 호출
    {
        GameManager.instance.ReLoadClip();
    }

    // 화면 중앙 목표점(크로스헤어) 계산
    private Vector3 GetShootTarget()
    {
        Transform camTransform = Camera.main != null ? Camera.main.transform : null;
        Vector3 targetPosition;
        if (camTransform != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, Mathf.Infinity, targetLayer))
                targetPosition = hit.point;
            else
                targetPosition = camTransform.position + camTransform.forward * aimObjDis;

            // ★ 화면 중앙 레이 시각화
            Debug.DrawLine(camTransform.position, targetPosition, Color.cyan, 0.02f, false);
        }
        else
        {
            targetPosition = transform.position + transform.forward * aimObjDis;
        }

        if (aimObj != null) aimObj.transform.position = targetPosition;
        return targetPosition;
    }

    // 카메라 앞 약간 떨어진 위치(자기 몸 충돌 방지)
    private Vector3 GetCameraSpawnOrigin()
    {
        var cam = Camera.main ? Camera.main.transform : null;
        if (!cam) return transform.position + transform.forward * 0.5f;
        return cam.position + cam.forward * cameraSpawnOffset;
    }

    // 카메라에서 시작해도 되는지(총구-카메라 사이에 벽이 막고 있나?) 간단 배럴체크
    private bool CanUseCameraOrigin(Vector3 camOrigin)
    {
        // 총구가 없으면 그냥 허용
        var gm = GameManager.instance;
        var w = gm ? gm.GetCurrentWeapon() : null;
        if (w == null || w.BulletPoint == null) return true;

        Vector3 p0 = w.BulletPoint.position;
        Vector3 p1 = camOrigin;
        Vector3 dir = (p1 - p0);
        float dist = dir.magnitude;
        if (dist <= 0.001f) return true;
        dir /= dist;

        // 총구에서 카메라 원점까지 가로막는 물체가 있으면 카메라 원점 사용 금지
        return !Physics.Raycast(p0, dir, dist, obstacleMask, QueryTriggerInteraction.Ignore);
    }

    // 발사 가능 여부(간단 래퍼)
    private bool CanShootThisFrame()
    {
        var w = GameManager.instance != null ? GameManager.instance.GetCurrentWeapon() : null;
        if (w == null) return false;
        return (zoomOn || input.adsHolding) && !controller.isReLoad && w.CanShootNow;
    }
}
