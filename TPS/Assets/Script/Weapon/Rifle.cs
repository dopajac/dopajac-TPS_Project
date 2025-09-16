using System.Collections.Generic;
using UnityEngine;
public class Rifle : Weapon
{
    protected override void Awake()
    {
        weaponType = WeaponType.Rifle;
        allowedSlots = new List<AttachmentSlot>
        {
            AttachmentSlot.Scope, AttachmentSlot.Magazine, AttachmentSlot.Stock,
            AttachmentSlot.Grip, AttachmentSlot.Muzzle
        };
        baseDamage = 28f; baseMagCapacity = 30; baseSpread = 1.2f; baseRecoil = 1.2f; shotsPerSecond = 9f;
        baseBulletSpeed = 900f;
        base.Awake();
    }
}
