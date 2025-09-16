using System.Collections.Generic;
using UnityEngine;


public class Sniper : Weapon
{
    protected override void Awake()
    {
        weaponType = WeaponType.Sniper;
        allowedSlots = new List<AttachmentSlot>
        {
            AttachmentSlot.Scope, AttachmentSlot.Magazine, AttachmentSlot.Stock, AttachmentSlot.Muzzle
        };
        baseDamage = 80f; baseMagCapacity = 5; baseSpread = 0.5f; baseRecoil = 2.0f; shotsPerSecond = 1.1f;
        baseBulletSpeed = 1200f;
        base.Awake();
    }
}