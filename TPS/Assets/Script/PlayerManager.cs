using System;
using StarterAssets;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UIElements;

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
    [SerializeField] private float aimObjDis;
    [SerializeField] private Cinemachine3rdPersonFollow aimCam_cm;
    

    [Header("Zoom (Tap)")]
    [SerializeField] private float zoom_cm_Distance = -5f; // 줌 시 카메라 거리(양수)
    private float base_cm_Distance;                          // 기본 거리
    private bool zoomOn;
    
    [Header("IK")] 
    [SerializeField]private Rig handRig;
    [SerializeField]private Rig aimRig;
    
    // === 추가: 차징 -> 점프높이 매핑 ===
    [Header("Charged Jump")]
    [SerializeField] private float minJumpHeight = 0.8f;   // 아주 짧게 눌러도 이 정도는 뜀
    [SerializeField] private float maxJumpHeight = 9.0f;   // 풀차지 점프 높이
    [SerializeField] private AnimationCurve chargeToHeight = AnimationCurve.EaseInOut(0,0, 1,1);
    // 커브로 초반 급히 오르고 후반 완만 등 원하는 감각 조절
    
    private void Start()
    {
        input = GetComponent<StarterAssetsInputs>();
        controller = GetComponent<ThirdPersonController>();
        if (aimCam_cm == null && aimCam != null)
            aimCam_cm = aimCam.GetComponent<Cinemachine3rdPersonFollow>();

        if (aimCam_cm != null)
            base_cm_Distance = aimCam_cm.CameraDistance;
        anim = GetComponent<Animator>();
        
        ApplyZoom(); // 초기화
        
    }

    private void ApplyZoom()
    {
        if (aimCam_cm == null) return;
        aimCam_cm.CameraDistance = zoomOn ? zoom_cm_Distance : base_cm_Distance;
    }
    
    private void Update()
    {
        // 1) Tap 신호 소비해서 줌 토글
        if (input.ConsumeZoomTap())
        {
            zoomOn = !zoomOn;
            ApplyZoom();
        }

        // 2) 견착/조준 실행
        AimCheck();
        
        if (input.ConsumeChargeReleased(out float chargeSec))
        {
            float t = (input.chargeMax > 0f) ? Mathf.Clamp01(chargeSec / input.chargeMax) : 0f;

            // 네가 지정한 커브/최소/최대 높이로 매핑 (예: 0.8m ~ 3.0m)
            float h01 = Mathf.Clamp01(chargeToHeight.Evaluate(t));
            float jumpHeight = Mathf.Lerp(minJumpHeight, maxJumpHeight, h01);

            controller.ChargedJump(jumpHeight);
        }
        
    }

    private void AimCheck()
    {
        // 0) 리로드 처리
        if (input.reload)
        {
            input.reload = false;

            if (controller.isReLoad) return;

            AimControll(false);
            SetRigWeight(0);
            anim.SetLayerWeight(1, 1);
            anim.SetTrigger("Reload");
            controller.isReLoad = true;
        }

        if (controller.isReLoad) return;

        // 1) 이번 프레임의 공통 타겟 포인트 계산
        Vector3 targetPosition = GetShootTarget();

        if (input.adsHolding)
        {
            // 견착 조준
            AimControll(true);
            anim.SetLayerWeight(1, 1);
            SetRigWeight(1);

            // 캐릭터를 타겟 수평 방향으로 회전
            Vector3 targetFlat = targetPosition; 
            targetFlat.y = transform.position.y;
            Vector3 aimDir = (targetFlat - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * 50f);

            // 발사
            if (input.shoot && CanShootThisFrame())
            {
                anim.SetBool("Shoot", true);
                GameManager.instance.Shooting(targetPosition);
            }
            else
            {
                anim.SetBool("Shoot", false);
            }
        }
        else
        {
            // 비조준(기본) 상태
            AimControll(false);
            SetRigWeight(0);
            anim.SetLayerWeight(1, 0);

            // 기본 상태라도 "줌만 켜짐"이면 CanShootThisFrame()이 true가 됨 → 발사 허용
            if (input.shoot && CanShootThisFrame())
            {
                // 필요시 살짝만 타겟 방향으로 몸 돌려줌
                Vector3 targetFlat = targetPosition; 
                targetFlat.y = transform.position.y;
                Vector3 aimDir = (targetFlat - transform.position).normalized;
                transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * 15f);

                anim.SetBool("Shoot", true);
                GameManager.instance.Shooting(targetPosition);
            }
            else
            {
                anim.SetBool("Shoot", false);
            }
        }
    }


    private void AimControll(bool isCheck)
    {
        aimCam.gameObject.SetActive(isCheck);
        aimImage.SetActive(isCheck);
        controller.isAimMove = isCheck;
    }

    public void Reroad()
    {
        //Debug.Log("Reload");
        controller.isReLoad= false;
        SetRigWeight(1);
        anim.SetLayerWeight(1,0);
    }

    public void SetRigWeight(float weight)
    {
        aimRig.weight = weight;
        handRig.weight = weight; 
    }

    public void ReLoadWeaponClip()
    {
        GameManager.instance.ReLoadClip();
    }
    
    private Vector3 GetShootTarget()
    {
        Transform camTransform = Camera.main != null ? Camera.main.transform : null;
        Vector3 targetPosition;
        if (camTransform != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, Mathf.Infinity, targetLayer))
            {
                targetPosition = hit.point;
            }
            else
            {
                targetPosition = camTransform.position + camTransform.forward * aimObjDis;
            }
        }
        else
        {
            // 카메라가 없으면 대략 전방으로
            targetPosition = transform.position + transform.forward * aimObjDis;
        }

        if (aimObj != null) aimObj.transform.position = targetPosition;
        return targetPosition;
    }
    
    private bool CanShootThisFrame()
    {
        // 둘 중 하나라도 켜져 있으면(zoomOn 또는 adsHolding) + 리로드 중이 아닐 때만 발사
        return (zoomOn || input.adsHolding) && !controller.isReLoad;
    }
    
    
}
