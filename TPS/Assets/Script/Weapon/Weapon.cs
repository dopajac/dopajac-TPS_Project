using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Type")]
    [SerializeField] protected WeaponType weaponType;

    [Header("Base Stats")]
    [SerializeField] protected float baseDamage = 30f;
    [SerializeField] protected float baseBulletSpeed = 800f; // m/s 가정
    [SerializeField] protected float baseRecoil = 1f;
    [SerializeField] protected float baseSpread = 1f;
    [SerializeField] protected int   baseMagCapacity = 30;
    [SerializeField] protected float shotsPerSecond = 10f;

    [Header("Shooting Refs")]
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Transform bulletPoint;
    [SerializeField] protected GameObject weaponFlashFX;
    [SerializeField] protected Transform bulletCasePoint;
    [SerializeField] protected GameObject bulletCaseFX;
    [SerializeField] protected Transform weaponClipPoint;
    [SerializeField] protected GameObject weaponClipFX;
    [SerializeField] protected LayerMask hitLayerMask = ~0;
    public Transform BulletPoint => bulletPoint;
    [Header("Sockets (child transforms)")]
    [SerializeField] protected Transform scopeSocket;
    [SerializeField] protected Transform muzzleSocket;
    [SerializeField] protected Transform gripSocket;
    [SerializeField] protected Transform magSocket;
    [SerializeField] protected Transform stockSocket;

    [Header("Allowed Slots")]
    [SerializeField] protected List<AttachmentSlot> allowedSlots = new();

    // runtime stats
    protected float damage, bulletSpeed, recoil, spread;
    protected int   magCapacity;
    protected int   currentAmmo;
    protected float fireCooldown;

    protected Dictionary<AttachmentSlot, AttachmentSO> equipped = new();
    protected Dictionary<AttachmentSlot, GameObject> spawnedViews = new();

    public bool CanShootNow => fireCooldown <= 0f && currentAmmo > 0;

    protected virtual void Awake()
    {
        ResetStats();
        currentAmmo = magCapacity;
    }

    protected virtual void Update()
    {
        if (fireCooldown > 0f) fireCooldown -= Time.deltaTime;
    }

    protected void ResetStats()
    {
        damage = baseDamage;
        bulletSpeed = baseBulletSpeed;
        recoil = baseRecoil;
        spread = baseSpread;
        magCapacity = baseMagCapacity;
    }

    protected void ApplyStats()
    {
        ResetStats();
        foreach (var kv in equipped)
        {
            var a = kv.Value;
            damage      *= a.damageMul;
            bulletSpeed *= a.bulletSpeedMul;
            recoil      *= a.recoilMul;
            spread      *= a.spreadMul;
            magCapacity += a.magCapacityDelta;
        }
        magCapacity = Mathf.Max(1, magCapacity);
        currentAmmo = Mathf.Clamp(currentAmmo, 0, magCapacity);
    }

    public bool CanEquip(AttachmentSO att)
    {
        if (att == null) return false;
        return allowedSlots.Contains(att.slot);
    }

    public bool Equip(AttachmentSO att)
    {
        if (!CanEquip(att)) return false;

        if (equipped.ContainsKey(att.slot))
            Unequip(att.slot);

        equipped[att.slot] = att;
        SpawnView(att);
        ApplyStats();
        return true;
    }

    public void Unequip(AttachmentSlot slot)
    {
        if (!equipped.ContainsKey(slot)) return;
        DespawnView(slot);
        equipped.Remove(slot);
        ApplyStats();
    }

    protected void SpawnView(AttachmentSO att)
    {
        if (!att.viewPrefab) return;
        var socket = GetSocket(att.slot);
        if (!socket) return;
        var go = Instantiate(att.viewPrefab, socket);
        go.transform.localPosition = att.localPosOffset;
        go.transform.localEulerAngles = att.localEulerOffset;
        spawnedViews[att.slot] = go;
    }

    protected void DespawnView(AttachmentSlot slot)
    {
        if (spawnedViews.TryGetValue(slot, out var go) && go)
            Destroy(go);
        spawnedViews.Remove(slot);
    }

    protected Transform GetSocket(AttachmentSlot slot) => slot switch
    {
        AttachmentSlot.Scope    => scopeSocket,
        AttachmentSlot.Muzzle   => muzzleSocket,
        AttachmentSlot.Grip     => gripSocket,
        AttachmentSlot.Magazine => magSocket,
        AttachmentSlot.Stock    => stockSocket,
        _ => null
    };

    public ScopeKind? GetCurrentScope()
    {
        if (equipped.TryGetValue(AttachmentSlot.Scope, out var a)) return a.scopeKind;
        return null;
    }

    // 기본: 총구에서 발사
    public bool TryShoot(Vector3 worldTarget)
    {
        return TryShootFrom(bulletPoint ? bulletPoint.position : transform.position, worldTarget);
    }

    // ADS 시: 카메라 중앙(origin)에서 발사도 가능
    public bool TryShootFrom(Vector3 origin, Vector3 worldTarget)
    {
        if (fireCooldown > 0f) return false;
        if (currentAmmo <= 0)  return false;

        currentAmmo--;
        fireCooldown = 1f / Mathf.Max(0.01f, shotsPerSecond);

        Vector3 dir = (worldTarget - origin).normalized;
        //Debug.DrawRay(origin, dir * 5f, Color.yellow, 1f, false); // 길이 5m, 1초 유지
        
        // FX (소염/소음 체크로 억제 가능)
        if (!IsFlashSuppressed())
        {
            if (weaponFlashFX && bulletPoint) Instantiate(weaponFlashFX, bulletPoint.position, bulletPoint.rotation);
            if (bulletCaseFX && bulletCasePoint) Instantiate(bulletCaseFX, bulletCasePoint.position, bulletCasePoint.rotation);
        }

        if (bulletPrefab)
        {
            var go = Instantiate(bulletPrefab, origin, Quaternion.LookRotation(dir, Vector3.up));
            var rb = go.GetComponent<Rigidbody>();
            if (rb) rb.AddForce(dir * bulletSpeed, ForceMode.VelocityChange);
        }
        return true;
    }

    protected bool IsFlashSuppressed()
    {
        foreach (var a in equipped.Values)
            if (a.suppressFlash) return true;
        return false;
    }

    protected bool IsSoundSuppressed()
    {
        foreach (var a in equipped.Values)
            if (a.suppressSound) return true;
        return false;
    }

    public void Reload()
    {
        if (weaponClipFX && weaponClipPoint) Instantiate(weaponClipFX, weaponClipPoint.position, weaponClipPoint.rotation);
        currentAmmo = magCapacity;
    }

    public int CurrentAmmo => currentAmmo;
    public int MagCapacity => magCapacity;

    private void OnValidate()
    {
        shotsPerSecond = Mathf.Max(0.01f, shotsPerSecond);
        baseMagCapacity = Mathf.Max(1, baseMagCapacity);
        baseBulletSpeed = Mathf.Max(0f, baseBulletSpeed);
    }
}
