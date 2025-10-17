using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Type")]
    [SerializeField]
    public WeaponType weaponType;

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
    
    [Header("=== 테스트 전용 ===")]
    [SerializeField] private AttachmentSO testAttachment; // 플레이 모드에서 드래그해서 넣을 SO
    private AttachmentSO _lastTestAttachment;

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
        if (!bulletPoint)
            bulletPoint = FindInScene("BulletPoint");

        if (!bulletCasePoint)
            bulletCasePoint = FindInScene("BulletCasePoint");

        if (!weaponClipPoint)
            weaponClipPoint = FindInScene("ClipPoint");
        
        ResetStats();
        currentAmmo = magCapacity;
    }

    protected virtual void Update()
    {
        if (fireCooldown > 0f) fireCooldown -= Time.deltaTime;

        // 플레이 모드에서 testAttachment 바뀌면 자동 장착
        if (Application.isPlaying && testAttachment != _lastTestAttachment)
        {
            _lastTestAttachment = testAttachment;

            if (_lastTestAttachment != null)
            {
                Equip(_lastTestAttachment);   // 효과만 반영됨 (viewPrefab null이면 외형은 안 바뀜)
                Debug.Log($"[테스트] {_lastTestAttachment.id} 장착됨: Damage={damage}, Mag={magCapacity}");
            }
        }
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
    public virtual bool TryShoot(Vector3 worldTarget)
    {
        if (fireCooldown > 0f) return false;
        if (currentAmmo <= 0) return false;

        currentAmmo--;
        fireCooldown = 1f / Mathf.Max(0.01f, shotsPerSecond);

        // fx
        if (bulletCaseFX && bulletCasePoint) Instantiate(bulletCaseFX, bulletCasePoint.position, bulletCasePoint.rotation);
        if (weaponFlashFX && bulletPoint)    Instantiate(weaponFlashFX, bulletPoint.position, bulletPoint.rotation);

        // === Spread (탄퍼짐) 적용 ===
        Vector3 aimDir = (worldTarget - bulletPoint.position).normalized;

        // 탄퍼짐을 구면 랜덤으로 추가
        if (spread > 0f)
        {
            // 작은 원뿔 범위 내에서 랜덤한 방향
            float spreadAngle = spread; // degree 단위로 쓴다고 가정
            aimDir = Quaternion.Euler(
                UnityEngine.Random.Range(-spreadAngle, spreadAngle),
                UnityEngine.Random.Range(-spreadAngle, spreadAngle),
                0f
            ) * aimDir;
        }

        // === Projectile 생성 ===
        if (bulletPrefab && bulletPoint)
        {
            var go = Instantiate(bulletPrefab, bulletPoint.position, Quaternion.LookRotation(aimDir, Vector3.up));
            var rb = go.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.linearVelocity = aimDir * bulletSpeed * 0.01f;
            }
        }

        // === Recoil (반동) 적용 ===
        if (recoil > 0f && Camera.main != null)
        {
            // 간단하게 카메라 pitch를 위로 튕기기
            // (실제로는 PlayerManager에 연결된 CameraController에 hook해서 처리하는 게 더 자연스러움)
            Transform cam = Camera.main.transform;
            cam.rotation *= Quaternion.Euler(-recoil, 0f, 0f); 
        }

        return true;
    }

    public void Reload()
    {
        if (weaponClipFX && weaponClipPoint) Instantiate(weaponClipFX, weaponClipPoint.position, weaponClipPoint.rotation);
        currentAmmo = magCapacity;
    }

    private Transform FindInScene(string name)
    {
        foreach (var go in GameObject.FindObjectsOfType<Transform>(true))
        {
            if (go.name == name)
                return go;
        }
        return null;
    }
    public bool AllowedSlotsContains(AttachmentSlot slot)
    {
        return allowedSlots.Contains(slot);
    }
    
    public int CurrentAmmo => currentAmmo;
    public int MagCapacity => magCapacity;
}
