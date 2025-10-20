using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

        // 현재 클릭된 버튼 가져오기
        Button clickedButton = EventSystem.current.currentSelectedGameObject?.GetComponent<Button>();
        if (clickedButton != null)
        {
            Image clickedImage = clickedButton.GetComponent<Image>();
            if (clickedImage != null)
            {
                // 🔹 mainButton의 ColorBlock을 수정
                var colors = mainButton.colors;
                colors.normalColor = clickedImage.color;  // 선택한 버튼의 색상으로 변경
                mainButton.colors = colors; // 구조체이므로 다시 할당 필수
            }
        }

        // TODO: 여기서 WeaponHolder / Weapon 에 옵션 적용
        // 예: weaponHolder.CurrentWeapon.Attach(AttachmentType.Muzzle, optionName);

        if (optionsPanel)
            optionsPanel.SetActive(false); // 선택 후 닫기
    }
}