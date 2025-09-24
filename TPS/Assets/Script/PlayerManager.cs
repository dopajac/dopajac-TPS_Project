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

    [Header("Weapon")]
    [SerializeField] private WeaponHolder weaponHolder;

    // 새로 추가: 액션 레이어 인덱스 자동 탐색
    private int actionLayerIndex = 1;

    private void Start()
    {
        input = GetComponent<StarterAssetsInputs>();
        controller = GetComponent<ThirdPersonController>();
        if (aimCam_cm == null && aimCam != null)
            aimCam_cm = aimCam.GetComponent<Cinemachine3rdPersonFollow>();
        if (aimCam_cm != null)
            base_cm_Distance = aimCam_cm.CameraDistance;

        anim = GetComponent<Animator>();
        if (anim != null)
        {
            int idx = anim.GetLayerIndex("Action Layer"); // 레이어 이름 정확히 일치해야 함
            if (idx >= 0) actionLayerIndex = idx;
        }

        if (!weaponHolder) weaponHolder = GetComponent<WeaponHolder>();

        ApplyZoom();                 // 카메라 적용
        ApplyAimVisuals();           // 처음 상태 반영
    }

    private void Update()
    {
        // 탭 줌 토글
        if (input.ConsumeZoomTap())
        {
            zoomOn = !zoomOn;
            ApplyZoom();
            ApplyAimVisuals();       // 줌 토글 시 애니메이션/리그도 즉시 동기화
        }

        AimCheck();

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

    // ⭐ 핵심: 줌이거나(zoomOn) 혹은 견착 중이면(adsHolding) 액션레이어/리그 ON
    private void ApplyAimVisuals()
    {
        bool aimingPose = (zoomOn || input.adsHolding) && !controller.isReLoad;
        SetRigWeight(aimingPose ? 1f : 0f);
        if (anim) anim.SetLayerWeight(actionLayerIndex, aimingPose ? 1f : 0f);
    }

    private void AimCheck()
    {
        // 리로드 시작
        if (input.reload)
        {
            input.reload = false;
            if (!controller.isReLoad)
            {
                AimControll(false);
                controller.isReLoad = true;
                if (anim)
                {
                    anim.SetLayerWeight(actionLayerIndex, 1f); // 리로드 클립이 액션 레이어에 있으면 가중치 유지
                    anim.SetTrigger("Reload");
                }
                SetRigWeight(0f); // 리로드 동안 리그 비활성(원하면 유지로 바꿔도 됨)
            }
        }

        // 리로드 중엔 조준/사격 금지, 비주얼만 유지
        if (controller.isReLoad)
        {
            if (anim) anim.SetBool("Shoot", false);
            return;
        }

        Vector3 targetPosition = GetShootTarget();

        if (input.adsHolding)
        {
            AimControll(true);
            ApplyAimVisuals(); // adsHolding=true 반영

            // 캐릭터 방향 보정
            Vector3 targetFlat = targetPosition; targetFlat.y = transform.position.y;
            Vector3 aimDir = (targetFlat - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * 50f);

            if (input.shoot && CanShootThisFrame())
            {
                weaponHolder.Shoot(targetPosition);
                if (anim) anim.SetBool("Shoot", true);
            }
            else if (anim) anim.SetBool("Shoot", false);
        }
        else
        {
            AimControll(false);
            ApplyAimVisuals(); // ⭐ 여기서 더 이상 레이어/리그를 0으로 내리지 않음(zoomOn 고려)

            if (input.shoot && CanShootThisFrame())
            {
                Vector3 targetFlat = targetPosition; targetFlat.y = transform.position.y;
                Vector3 aimDir = (targetFlat - transform.position).normalized;
                transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * 15f);

                weaponHolder.Shoot(targetPosition);
                if (anim) anim.SetBool("Shoot", true);
            }
            else if (anim) anim.SetBool("Shoot", false);
        }
    }

    private void AimControll(bool isCheck)
    {
        if (aimCam) aimCam.gameObject.SetActive(isCheck);
        if (aimImage) aimImage.SetActive(isCheck);
        controller.isAimMove = isCheck;
    }

    // 애니메이션 이벤트
    public void Reroad()
    {
        controller.isReLoad = false;
        ApplyAimVisuals();        // 리로드 끝나면 현재 zoom/ads 상태에 맞춰 복원
        if (anim) anim.SetLayerWeight(actionLayerIndex, (zoomOn || input.adsHolding) ? 1f : 0f);
        if (anim) anim.SetLayerWeight(1, anim.GetLayerWeight(actionLayerIndex)); // 혹시 하드코딩 되어 있을 수 있어 동기화
        if (anim) anim.SetBool("Shoot", false);
    }

    public void SetRigWeight(float weight)
    {
        if (aimRig)  aimRig.weight  = weight;
        if (handRig) handRig.weight = weight;
    }

    public void ReLoadWeaponClip() // 애니메이션 이벤트에서 호출
    {
        if (weaponHolder) weaponHolder.Reload();
    }

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
        }
        else
        {
            targetPosition = transform.position + transform.forward * aimObjDis;
        }

        if (aimObj != null) aimObj.transform.position = targetPosition;
        return targetPosition;
    }

    private bool CanShootThisFrame()
    {
        if (!weaponHolder || !weaponHolder.HasWeapon) return false;
        return (zoomOn || input.adsHolding) && !controller.isReLoad && weaponHolder.CanShootNow;
    }
}
