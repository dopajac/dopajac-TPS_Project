using UnityEngine;
using UnityEngine.UI;
using System;

public class WeaponHolder : MonoBehaviour
{
    [Header("Slots (0 = Key 1, 1 = Key 2)")]
    [SerializeField] private Weapon[] slots = new Weapon[2];
    [SerializeField] private int activeSlot = -1; // -1 = unarmed

    [Header("UI (optional)")]
    [SerializeField] private Text bulletCountText;

    public event Action<int, Weapon> OnWeaponSwitched;

    public int ActiveSlot => activeSlot;
    public Weapon CurrentWeapon => (activeSlot >= 0 && activeSlot < slots.Length) ? slots[activeSlot] : null;
    public bool HasWeapon => CurrentWeapon != null;
    public bool CanShootNow => CurrentWeapon && CurrentWeapon.CanShootNow;

    private void OnEnable()
    {
        UpdateActiveVisibility();
        UpdateAmmoUI();
    }

    private void Update()
    {
        UpdateAmmoUI();
    }

    private void UpdateAmmoUI()
    {
        if (!bulletCountText) return;

        if (CurrentWeapon)
            bulletCountText.text = CurrentWeapon.CurrentAmmo + " / " + CurrentWeapon.MagCapacity;
        else
            bulletCountText.text = "-- / --";
    }

    public void SetWeapon(int slotIndex, Weapon w)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length) return;
        slots[slotIndex] = w;

        if (w) w.gameObject.SetActive(false);

        if (activeSlot == -1 && w != null)
            SwitchTo(slotIndex);
    }

    public Weapon GetWeapon(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length) return null;
        return slots[slotIndex];
    }

    public void SwitchTo(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length) return;

        activeSlot = (slots[slotIndex] != null) ? slotIndex : -1;
        UpdateActiveVisibility();

        // 무기가 있으면 그대로, 없으면 null 전달
        OnWeaponSwitched?.Invoke(activeSlot, CurrentWeapon);
    }

    public void GoUnarmed()
    {
        activeSlot = -1;
        UpdateActiveVisibility();
        OnWeaponSwitched?.Invoke(activeSlot, null);
    }

    public void CycleNext()
    {
        if (slots.Length == 0) { GoUnarmed(); return; }
        for (int i = 1; i <= slots.Length; i++)
        {
            int next = (activeSlot + i) % slots.Length;
            if (slots[next] != null) { SwitchTo(next); return; }
        }
        GoUnarmed();
    }

    public void Shoot(Vector3 targetPosition)
    {
        if (CurrentWeapon) CurrentWeapon.TryShoot(targetPosition);
    }

    public void Reload()
    {
        if (CurrentWeapon) CurrentWeapon.Reload();
        UpdateAmmoUI();
    }

    private void UpdateActiveVisibility()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i]) continue;
            bool active = (i == activeSlot);
            slots[i].gameObject.SetActive(active);
        }
        UpdateAmmoUI();
    }
}
