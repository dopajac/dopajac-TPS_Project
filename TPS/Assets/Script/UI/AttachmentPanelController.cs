using UnityEngine;

public class AttachmentPanelController : MonoBehaviour
{
    [Header("Attachment Panels")]
    public GameObject scopePanel;
    public GameObject muzzlePanel;
    public GameObject gripPanel;
    public GameObject magPanel;
    public GameObject stockPanel;

    [Header("Refs")]
    public WeaponHolder holder;  // 플레이어의 WeaponHolder 연결
    private int activeSlotIndex = -1; // 현재 제어 중인 슬롯 인덱스 (0 or 1)
    
    /// <summary>
    /// WeaponData.weaponType 에 따라 패널 On/Off
    /// </summary>
    ///
    public void SetActiveSlot(int index)
    {
        activeSlotIndex = index; // 현재 무기 슬롯 번호 저장
        Debug.Log($"[AttachmentPanel] 현재 활성 무기 슬롯: {activeSlotIndex}");
    }

    public void RefreshUI(WeaponType type)
    {
        // 기본적으로 전부 끔
        scopePanel.SetActive(false);
        muzzlePanel.SetActive(false);
        gripPanel.SetActive(false);
        magPanel.SetActive(false);
        stockPanel.SetActive(false);

        switch (type)
        {
            case WeaponType.Rifle:
                scopePanel.SetActive(true);
                muzzlePanel.SetActive(true);
                gripPanel.SetActive(true);
                magPanel.SetActive(true);
                stockPanel.SetActive(true);
                break;

            case WeaponType.Shotgun:
                magPanel.SetActive(true);
                stockPanel.SetActive(true);
                break;

            case WeaponType.Sniper:
                scopePanel.SetActive(true);
                muzzlePanel.SetActive(true);
                magPanel.SetActive(true);
                stockPanel.SetActive(true);
                break;

            /*case WeaponType.Pistol:
                muzzlePanel.SetActive(true);
                magPanel.SetActive(true);
                break;*/

            // 필요하면 SMG, AR, DMR 등 다른 타입 추가 가능
        }
    }
}