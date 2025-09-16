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

    private void Start()
    {
        input = GetComponent<StarterAssetsInputs>();
        controller = GetComponent<ThirdPersonController>();
        if (aimCam_cm == null && aimCam != null)
            aimCam_cm = aimCam.GetComponent<Cinemachine3rdPersonFollow>();

        if (aimCam_cm != null)
            base_cm_Distance = aimCam_cm.CameraDistance;

        anim = GetComponent<Animator>();

        if (!weaponHolder) weaponHolder = GetComponent<WeaponHolder>();

        ApplyZoom();
    }

    private void Update()
    {
        if (input.ConsumeZoomTap())
        {
            zoomOn = !zoomOn;
            ApplyZoom();
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

    private void AimCheck()
    {
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
            AimControll(true);
            anim.SetLayerWeight(1, 1);
            SetRigWeight(1);

            Vector3 targetFlat = targetPosition;
            targetFlat.y = transform.position.y;
            Vector3 aimDir = (targetFlat - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * 50f);

            if (input.shoot && CanShootThisFrame())
            {
                weaponHolder.Shoot(targetPosition);
                anim.SetBool("Shoot", true);
            }
            else anim.SetBool("Shoot", false);
        }
        else
        {
            AimControll(false);
            SetRigWeight(0);
            anim.SetLayerWeight(1, 0);

            if (input.shoot && CanShootThisFrame())
            {
                Vector3 targetFlat = targetPosition;
                targetFlat.y = transform.position.y;
                Vector3 aimDir = (targetFlat - transform.position).normalized;
                transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * 15f);

                weaponHolder.Shoot(targetPosition);
                anim.SetBool("Shoot", true);
            }
            else anim.SetBool("Shoot", false);
        }
    }

    private void AimControll(bool isCheck)
    {
        if (aimCam) aimCam.gameObject.SetActive(isCheck);
        if (aimImage) aimImage.SetActive(isCheck);
        controller.isAimMove = isCheck;
    }

    // Animation Event
    public void Reroad()
    {
        controller.isReLoad = false;
        SetRigWeight(1);
        anim.SetLayerWeight(1, 0);
    }

    public void SetRigWeight(float weight)
    {
        if (aimRig) aimRig.weight = weight;
        if (handRig) handRig.weight = weight;
    }

    // Animation Event
    public void ReLoadWeaponClip()
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
