using UnityEngine;

public class AttachmentPanelController : MonoBehaviour
{
    [Header("Attachment Panels")]
    public GameObject scopePanel;
    public GameObject muzzlePanel;
    public GameObject gripPanel;
    public GameObject magPanel;
    public GameObject stockPanel;

    /// <summary>
    /// WeaponData.weaponType 에 따라 패널 On/Off
    /// </summary>
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