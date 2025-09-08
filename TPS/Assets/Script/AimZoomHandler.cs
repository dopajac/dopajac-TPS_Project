using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Unity.Cinemachine;

public class AimZoomHandler : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference aimAction; // Player/Aim 액션

    [Header("Cams")]
    public Cinemachine3rdPersonFollow baseCam_am; // 평상시 카메라
    public CinemachineVirtualCamera aimCam;  // 견착 카메라(조준 시 활성)

    [Header("Zoom")]
    public float normalFov = -5f;
    public float zoomFov = 40f;
    private bool zoomOn;
    private bool isAiming;

    private void OnEnable() {
        var a = aimAction.action;
        a.Enable();
        a.started   += OnStarted;
        a.performed += OnPerformed;
        a.canceled  += OnCanceled;
    }
    private void OnDisable() {
        var a = aimAction.action;
        a.started   -= OnStarted;
        a.performed -= OnPerformed;
        a.canceled  -= OnCanceled;
        a.Disable();
    }

    private void OnStarted(InputAction.CallbackContext ctx) {
        Debug.Log("RMB Down (started)");
    }

    private void OnPerformed(InputAction.CallbackContext ctx) {
        if (ctx.interaction is TapInteraction) {
            Debug.Log("RMB Tap (performed at release)");
            zoomOn = !zoomOn;
            ApplyFov(zoomOn ? zoomFov : normalFov);   // 둘 다 적용
        }
        else if (ctx.interaction is HoldInteraction) {
            Debug.Log("RMB Hold Reached (performed while pressed)");
            isAiming = true;
            // PlayerManager가 input.aim을 보고 aimCam 활성화하므로 그대로 사용
            if (input) input.aim = true;
            // 혹시 이미 줌 On이면 에임 카메라에도 FOV 즉시 반영
            if (zoomOn) ApplyFov(zoomFov);
        }
    }

    private void OnCanceled(InputAction.CallbackContext ctx) {
        Debug.Log("RMB Up (canceled)");
        if (isAiming) {
            isAiming = false;
            if (input) input.aim = false;
            // 줌 상태 유지/해제는 디자인에 맞게. 유지하려면 아무 것도 안 해도 됨.
            if (zoomOn) ApplyFov(zoomFov); else ApplyFov(normalFov);
        }
    }

    [Header("Refs")]
    public StarterAssets.StarterAssetsInputs input;

    private void ApplyFov(float fov) {
        //if (baseCam_am) baseCam_am.m_Lens.FieldOfView = fov;
        if (aimCam)  aimCam.m_Lens.FieldOfView  = fov;
        Debug.Log($"ApplyFov -> {fov}");
    }
}
