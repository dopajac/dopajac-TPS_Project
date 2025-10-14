using UnityEngine;

public class PanelOnOff : MonoBehaviour
{
    [SerializeField] private GameObject inventorypanel;
    [SerializeField] private GameObject MapListpanel;
    
    [SerializeField] private StarterAssets.StarterAssetsInputs input; // StarterAssetsInputs 연결

    private void Update()
    {
        InventoryUIOnOff();
        MapListUIOnOff();
    }

    private void InventoryUIOnOff()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            bool active = !inventorypanel.activeSelf;
            inventorypanel.SetActive(active);

            if (active)
            {
                // 인벤토리 켜짐 → 마우스 보이게, 카메라 입력 차단
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                if (input) input.cursorInputForLook = false;
            }
            else
            {
                // 인벤토리 꺼짐 → 마우스 잠금, 카메라 입력 재개
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                if (input) input.cursorInputForLook = true;
            }
        }
    }
    private void MapListUIOnOff()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            bool active = !MapListpanel.activeSelf;
            MapListpanel.SetActive(active);

            if (active)
            {
                // 인벤토리 켜짐 → 마우스 보이게, 카메라 입력 차단
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                if (input) input.cursorInputForLook = false;
            }
            else
            {
                // 인벤토리 꺼짐 → 마우스 잠금, 카메라 입력 재개
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                if (input) input.cursorInputForLook = true;
            }
        }
    }
}