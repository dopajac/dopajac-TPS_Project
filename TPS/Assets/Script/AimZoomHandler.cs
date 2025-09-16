using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Unity.Cinemachine;

public class AimZoomHandler : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference aimAction; // Player/Aim 액션

    [Header("Cams")]
    public Cinemachine3rdPersonFollow baseCam_cm; // 기본 카메라 follow component
    public CinemachineVirtualCamera   aimCam;     // ADS 카메라

    [Header("Zoom by Distance (Tap)")]
    public float normalDistance = 4.5f;
    public float zoomDistance   = 2.5f;

    [Header("FOV by Scope (ADS)")]
    public float fovHip   = 70f;
    public float fovDot   = 60f;
    public float fov2x    = 50f;
    public float fov4x    = 40f;
    public float fov8x    = 30f;

    [Header("Refs")]
    public StarterAssets.StarterAssetsInputs input;
    public Weapon currentWeapon; // 현재 무기(스코프 확인용)

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

    private void Start()
    {
        ApplyDistance();
        ApplyFov(fovHip);
    }

    private void OnStarted(InputAction.CallbackContext ctx) { }
    private void OnPerformed(InputAction.CallbackContext ctx)
    {
        if (ctx.interaction is TapInteraction)
        {
            zoomOn = !zoomOn;
            ApplyDistance();
        }
        else if (ctx.interaction is HoldInteraction)
        {
            isAiming = true;
            if (input) input.aim = true;
            aimCam.gameObject.SetActive(true);
            ApplyFov( GetScopeFov() );
        }
        else
        {
            // 인터랙션 미설정 시 Tap처럼
            zoomOn = !zoomOn;
            ApplyDistance();
        }
    }

    private void OnCanceled(InputAction.CallbackContext ctx)
    {
        if (isAiming)
        {
            isAiming = false;
            if (input) input.aim = false;
            aimCam.gameObject.SetActive(false);
            ApplyFov(fovHip);
        }
    }

    private void ApplyDistance()
    {
        if (baseCam_cm) baseCam_cm.CameraDistance = zoomOn ? zoomDistance : normalDistance;
    }

    private float GetScopeFov()
    {
        if (!currentWeapon) return fovHip;
        switch (currentWeapon.GetCurrentScope())
        {
            case ScopeKind.RedDot: return fovDot;
            case ScopeKind.X2:     return fov2x;
            case ScopeKind.X4:     return fov4x;
            case ScopeKind.X8:     return fov8x;
            default:               return fovHip;
        }
    }

    private void ApplyFov(float fov)
    {
        if (aimCam) aimCam.m_Lens.FieldOfView = fov;
    }
}
