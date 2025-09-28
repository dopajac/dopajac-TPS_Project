using System.Collections.Generic;
using UnityEngine;

public class GunList : MonoBehaviour
{
    public WeaponDatabase database;        // 무기 데이터베이스 SO
    public GameObject GunListSlotPrefab;   // 슬롯 프리팹
    public Transform slotParent;           // 슬롯 부모 (GunChoose)

    private void Start()
    {
        foreach (var weaponData in database.weapons)
        {
            GameObject slotObj = Instantiate(GunListSlotPrefab, slotParent);

            GunListSlot slotScript = slotObj.GetComponent<GunListSlot>();
            if (slotScript != null)
            {
                slotScript.SetWeapon(weaponData);
            }
        }
    }
}