using UnityEngine;

public enum WeaponType { Shotgun, Rifle, Sniper }
public enum AttachmentSlot { Scope, Muzzle, Grip, Magazine, Stock }
public enum ScopeKind { RedDot, X2, X4, X8 }

[CreateAssetMenu(menuName = "TPS/Attachment")]
public class AttachmentSO : ScriptableObject
{
    public string id;
    public AttachmentSlot slot;

    [Header("View(optional)")]
    public GameObject viewPrefab;
    public Vector3 localPosOffset;
    public Vector3 localEulerOffset;

    [Header("Stat Modifiers")]
    public float damageMul = 1f;
    public float bulletSpeedMul = 1f;
    public float recoilMul = 1f;
    public float spreadMul = 1f;
    public float adsSpeedMul = 1f;
    public int   magCapacityDelta = 0;
    public bool  suppressFlash = false;
    public bool  suppressSound = false;

    [Header("Scope only")]
    public ScopeKind scopeKind;
}