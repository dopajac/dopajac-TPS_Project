using UnityEngine;
using UnityEngine.UI;

public class AttachmentButton : MonoBehaviour
{
    [Header("이 버튼이 대표하는 파츠")]
    public AttachmentSO attachment;

    private Button _button;
    private WeaponHolder _weaponHolder;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _weaponHolder = FindObjectOfType<WeaponHolder>();

        if (_button != null)
            _button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (attachment == null)
        {
            Debug.LogWarning($"{gameObject.name} 버튼에 AttachmentSO가 설정되지 않았음");
            return;
        }

        if (_weaponHolder == null || _weaponHolder.CurrentWeapon == null)
        {
            Debug.LogWarning("현재 장착된 무기가 없음 → 파츠 장착 불가");
            return;
        }

        Weapon weapon = _weaponHolder.CurrentWeapon;

        if (weapon.CanEquip(attachment))
        {
            weapon.Equip(attachment);
            Debug.Log($"{weapon.name} 에 {attachment.id} ({attachment.slot}) 장착됨");
        }
        else
        {
            Debug.LogWarning($"{weapon.name} 은 {attachment.slot} 슬롯 지원 안함");
        }
    }
}
