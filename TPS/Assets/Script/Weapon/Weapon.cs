using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Type")]
    [SerializeField] protected WeaponType weaponType;

    [Header("Base Stats")]
    [SerializeField] protected float baseDamage = 30f;
    [SerializeField] protected float baseBulletSpeed = 800f;
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

    [Header("Sockets (child transforms)")]
    [SerializeField] protected Transform scopeSocket;
    [SerializeField] protected Transform muzzleSocket;
    [SerializeField] protected Transform gripSocket;
    [SerializeField] protected Transform magSocket;
    [SerializeField] protected Transform stockSocket;

    [Header("Allowed Slots")]
    [SerializeField] protected List<AttachmentSlot> allowedSlots = new();

    public bool CanShootNow => fireCooldown <= 0f && currentAmmo > 0;
    
    // runtime stats
    protected float damage, bulletSpeed, recoil, spread;
    protected int   magCapacity;
    protected int   currentAmmo;
    protected float fireCooldown;

    protected Dictionary<AttachmentSlot, AttachmentSO> equipped = new();
    protected Dictionary<AttachmentSlot, GameObject> spawnedViews = new();

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

    // === Fire / Reload API (PlayerManager에서 호출) ===
    public bool TryShoot(Vector3 worldTarget)
    {
        if (fireCooldown > 0f) return false;
        if (currentAmmo <= 0) return false;

        currentAmmo--;
        fireCooldown = 1f / Mathf.Max(0.01f, shotsPerSecond);

        // fx
        if (bulletCaseFX && bulletCasePoint) Instantiate(bulletCaseFX, bulletCasePoint.position, bulletCasePoint.rotation);
        if (weaponFlashFX && bulletPoint)    Instantiate(weaponFlashFX, bulletPoint.position, bulletPoint.rotation);

        // projectile
        if (bulletPrefab && bulletPoint)
        {
            Vector3 aim = (worldTarget - bulletPoint.position).normalized;
            var go = Instantiate(bulletPrefab, bulletPoint.position, Quaternion.LookRotation(aim, Vector3.up));
            var rb = go.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.linearVelocity = aim * bulletSpeed * 0.01f; // bulletSpeed가 m/s라면 스케일 맞춰 조정
            }
        }

        return true;
    }

    public void Reload()
    {
        if (weaponClipFX && weaponClipPoint) Instantiate(weaponClipFX, weaponClipPoint.position, weaponClipPoint.rotation);
        currentAmmo = magCapacity;
    }

    public int CurrentAmmo => currentAmmo;
    public int MagCapacity => magCapacity;
}
