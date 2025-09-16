using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    protected override void Awake()
    {
        weaponType = WeaponType.Shotgun;
        allowedSlots = new List<AttachmentSlot>
        {
            AttachmentSlot.Magazine, AttachmentSlot.Stock
        };
        baseDamage = 12f; baseMagCapacity = 8; baseSpread = 2.5f; baseRecoil = 1.6f; shotsPerSecond = 1.5f;
        baseBulletSpeed = 500f;
        base.Awake();
    }
}
