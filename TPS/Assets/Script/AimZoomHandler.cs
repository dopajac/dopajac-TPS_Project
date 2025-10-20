using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Unity.Cinemachine;

public class AimZoomHandler : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference aimAction;

    [Header("Cams")]
    public CinemachineVirtualCamera followCam;
    public Cinemachine3rdPersonFollow baseCam_cm;
    public CinemachineVirtualCamera aimCam;

    [Header("Zoom by Distance (Tap)")]
    public float normalDistance = 4.5f;
    public float zoomDistance = 2.5f;

    [Header("FOV by Scope (ADS)")]
    public float fovHip = 70f;
    public float fovDot = 60f;
    public float fov2x = 50f;
    public float fov4x = 40f;
    public float fov8x = 30f;

    [Header("Refs")]
    public StarterAssets.StarterAssetsInputs input;
    public Weapon currentWeapon;
    [SerializeField] private WeaponHolder holder;

    [Header("UI")]
    public GameObject zoomAimUI;  // 👈 ZoomAim (1) 연결할 곳

    private bool zoomOn;
    private bool isAiming;

    private void OnEnable()
    {
        var a = aimAction.action;
        a.Enable();
        a.started += OnStarted;
        a.performed += OnPerformed;
        a.canceled += OnCanceled;
    }

    private void OnDisable()
    {
        var a = aimAction.action;
        a.started -= OnStarted;
        a.performed -= OnPerformed;
        a.canceled -= OnCanceled;
        a.Disable();
    }

    private void OnStarted(InputAction.CallbackContext ctx) { }

    private void OnPerformed(InputAction.CallbackContext ctx)
    {
        if (ctx.interaction is TapInteraction)
        {
            zoomOn = !zoomOn;
            ApplyDistance();

            // ✅ 오른쪽 클릭 “한 번” 눌렀을 때 UI 토글
            if (zoomAimUI)
                zoomAimUI.SetActive(zoomOn);

            Debug.Log($"[Zoom] Tap → ZoomAim UI {(zoomOn ? "활성화" : "비활성화")}");
        }
        else if (ctx.interaction is HoldInteraction)
        {
            isAiming = true;
            if (input) input.aim = true;
            if (aimCam) aimCam.gameObject.SetActive(true);
            ApplyFov(GetScopeFov());
        }
        else
        {
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
            if (aimCam) aimCam.gameObject.SetActive(false);
            ApplyFov(fovHip);
        }
    }

    private void ApplyDistance()
    {
        if (baseCam_cm)
            baseCam_cm.CameraDistance = zoomOn ? zoomDistance : normalDistance;
    }

    private float GetScopeFov()
    {
        var w = currentWeapon;
        if (!w && holder) w = holder.CurrentWeapon;
        if (!w) return fovHip;

        switch (w.GetCurrentScope())
        {
            case ScopeKind.RedDot: return fovDot;
            case ScopeKind.X2: return fov2x;
            case ScopeKind.X4: return fov4x;
            case ScopeKind.X8: return fov8x;
            default: return fovHip;
        }
    }

    private void ApplyFov(float fov)
    {
        if (followCam)
            followCam.m_Lens.FieldOfView = fov;
    }
}
