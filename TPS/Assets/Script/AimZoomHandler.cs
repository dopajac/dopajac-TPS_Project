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
    public CinemachineVirtualCamera   aimCam;

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
    public Weapon currentWeapon;                  // optional direct reference
    [SerializeField] private WeaponHolder holder; // preferred

    private bool zoomOn;
    private bool isAiming;

    private void OnEnable()
    {
        var a = aimAction.action;
        a.Enable();
        a.started   += OnStarted;
        a.performed += OnPerformed;
        a.canceled  += OnCanceled;
    }
    private void OnDisable()
    {
        var a = aimAction.action;
        a.started   -= OnStarted;
        a.performed -= OnPerformed;
        a.canceled  -= OnCanceled;
        a.Disable();
    }

    private void Start()
    {
        if (!holder) holder = GetComponentInParent<WeaponHolder>();
        if (!currentWeapon && holder) currentWeapon = holder.CurrentWeapon;

        if (holder) holder.OnWeaponSwitched += OnWeaponSwitched;

        ApplyDistance();
        ApplyFov(fovHip);
    }

    private void OnDestroy()
    {
        if (holder) holder.OnWeaponSwitched -= OnWeaponSwitched;
    }

    private void OnWeaponSwitched(int slot, Weapon w)
    {
        currentWeapon = w;
        if (isAiming)
        {
            ApplyFov(GetScopeFov());
        }
    }

    private void OnStarted(InputAction.CallbackContext ctx) { }

    private void OnPerformed(InputAction.CallbackContext ctx)
    {
        if (ctx.interaction is TapInteraction)
        {
            // 줌 토글
            zoomOn = !zoomOn;
            ApplyDistance();

            if (zoomOn)
            {
                ApplyFov(GetScopeFov());  // 배율 적용
                Debug.Log("[AimZoomHandler] Tap Zoom ON, FOV=" + followCam.m_Lens.FieldOfView);
            }
            else
            {
                ApplyFov(fovHip);        // 힙 파이어 상태
                Debug.Log("[AimZoomHandler] Tap Zoom OFF, FOV=" + followCam.m_Lens.FieldOfView);
            }
        }
        else if (ctx.interaction is HoldInteraction)
        {
            // 견착 시작
            isAiming = true;
            if (input) input.aim = true;
            if (aimCam) aimCam.gameObject.SetActive(true);
            ApplyFov(GetScopeFov());
            Debug.Log("[AimZoomHandler] Hold ADS, FOV=" + followCam.m_Lens.FieldOfView);
        }
        else
        {
            // 기타 인터랙션 → Tap처럼 처리
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
            ApplyFov(fovHip);
        }
    }

    private void ApplyDistance()
    {
        if (baseCam_cm) baseCam_cm.CameraDistance = zoomOn ? zoomDistance : normalDistance;
    }

    private float GetScopeFov()
    {
        var w = currentWeapon;
        if (!w && holder) w = holder.CurrentWeapon;
        if (!w) return fovHip;

        switch (w.GetCurrentScope())
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
        if (followCam)
        {
            followCam.m_Lens.FieldOfView = fov;
            Debug.Log($"[AimZoomHandler] ApplyFov: {fov}");
        }
        else
        {
            Debug.LogWarning("[AimZoomHandler] followCam이 비어있음!");
        }
    }
}
