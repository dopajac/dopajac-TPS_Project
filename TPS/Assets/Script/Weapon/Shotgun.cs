using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    [SerializeField] private int pelletsPerShot = 5; // 펠릿 개수

    protected override void Awake()
    {
        weaponType = WeaponType.Shotgun;
        allowedSlots = new List<AttachmentSlot>
        {
            AttachmentSlot.Magazine, AttachmentSlot.Stock
        };
        baseDamage = 12f; 
        baseMagCapacity = 40; 
        baseSpread = 5f;       // 퍼짐을 더 크게
        baseRecoil = 2.5f;     // 반동 크게
        shotsPerSecond = 1.0f; // 발사속도 느리게
        baseBulletSpeed = 500f;
        base.Awake();
    }

    public override bool TryShoot(Vector3 worldTarget)
    {
        if (fireCooldown > 0f) return false;
        if (currentAmmo <= 0) return false;

        currentAmmo--;
        fireCooldown = 1f / Mathf.Max(0.01f, shotsPerSecond);

        // fx
        if (bulletCaseFX && bulletCasePoint) 
            Instantiate(bulletCaseFX, bulletCasePoint.position, bulletCasePoint.rotation);
        if (weaponFlashFX && bulletPoint)    
            Instantiate(weaponFlashFX, bulletPoint.position, bulletPoint.rotation);

        // === 펠릿 여러 개 발사 ===
        for (int i = 0; i < pelletsPerShot; i++)
        {
            Vector3 aimDir = (worldTarget - bulletPoint.position).normalized;

            if (spread > 0f)
            {
                float spreadAngle = spread;
                aimDir = Quaternion.Euler(
                    Random.Range(-spreadAngle, spreadAngle),
                    Random.Range(-spreadAngle, spreadAngle),
                    0f
                ) * aimDir;
            }

            if (bulletPrefab && bulletPoint)
            {
                var go = Instantiate(bulletPrefab, bulletPoint.position, Quaternion.LookRotation(aimDir, Vector3.up));
                var rb = go.GetComponent<Rigidbody>();
                if (rb)
                {
                    rb.linearVelocity = aimDir * bulletSpeed * 0.01f;
                }
            }
        }

        // === Recoil ===
        if (recoil > 0f && Camera.main != null)
        {
            Transform cam = Camera.main.transform;
            cam.rotation *= Quaternion.Euler(-recoil, 0f, 0f);
        }

        return true;
    }
}
