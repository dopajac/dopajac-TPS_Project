using UnityEngine;
using UnityEngine.UI;

public class AttachmentListSlotOnOff : MonoBehaviour
{
    [Header("UI 연결")]
    public Button mainButton;          // Scope, Muzzle 같은 메인 버튼
    public GameObject optionsPanel;    // Scroll View (리스트)

    private void Awake()
    {
        if (mainButton)
            mainButton.onClick.AddListener(ToggleOptions);

        if (optionsPanel)
            optionsPanel.SetActive(false); // 시작은 꺼둠
    }

    private void ToggleOptions()
    {
        if (optionsPanel)
            optionsPanel.SetActive(!optionsPanel.activeSelf);
    }

    // 리스트 버튼에서 이 함수를 호출하도록 이벤트 연결
    public void OnOptionSelected(string optionName)
    {
        Debug.Log($"{mainButton.name} → {optionName} 선택됨");

        // TODO: 여기서 WeaponHolder / Weapon 에 옵션 적용
        // 예: 무기.Euqip(해당 AttachmentSO)

        if (optionsPanel)
            optionsPanel.SetActive(false); // 선택 후 닫기
    }
}
