using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GunInventorySlot : MonoBehaviour, IDropHandler
{
    [Header("UI 연결")]
    //public RawImage renderImage;   // 이 슬롯의 WeaponPreview RawImage

    private Camera renderCamera;
    private GameObject previewInstance;
    private RenderTexture rt;
    private Transform previewRoot;

    private WeaponData equippedWeapon;

    public void OnDrop(PointerEventData eventData)
    {
        if (DragDropManager.draggedWeapon != null && DragDropManager.draggedPreview != null)
        {
            // Preview를 이 슬롯 밑으로 이동
            DragDropManager.draggedPreview.transform.SetParent(transform, false);

            // 위치 중앙 정렬
            DragDropManager.draggedPreview.rectTransform.anchoredPosition = Vector2.zero;

            // Slot의 renderImage 갱신
            //renderImage = DragDropManager.draggedPreview;

            // 장착 무기 데이터 저장
            equippedWeapon = DragDropManager.draggedWeapon;

            Debug.Log($"{gameObject.name} 슬롯에 {equippedWeapon.weaponName} 장착됨");

            // 드래그 매니저 정리
            DragDropManager.draggedPreview = null;
            DragDropManager.draggedWeapon = null;
        }
    }

    public void SetWeapon(WeaponData weaponData)
    {
        equippedWeapon = weaponData;

        // RenderTexture 새로 생성
        rt = new RenderTexture(512, 512, 16);
        //renderImage.texture = rt;

        // 카메라 생성
        if (renderCamera == null)
        {
            GameObject camObj = new GameObject("RenderCam_" + weaponData.name);
            camObj.transform.SetParent(transform, false);
            camObj.transform.localPosition = new Vector3(0, 0, -2.5f);

            renderCamera = camObj.AddComponent<Camera>();
            renderCamera.clearFlags = CameraClearFlags.SolidColor;
            renderCamera.backgroundColor = Color.clear;
            renderCamera.orthographic = false;
            renderCamera.cullingMask = LayerMask.GetMask("UIWeapon");
            renderCamera.targetTexture = rt;
        }
        else
        {
            renderCamera.targetTexture = rt;
        }

        // 프리뷰 Root 생성
        if (previewRoot == null)
        {
            GameObject rootObj = new GameObject("PreviewRoot_" + weaponData.name);
            rootObj.transform.SetParent(transform, false);
            previewRoot = rootObj.transform;
        }

        // 기존 무기 제거
        if (previewInstance != null) Destroy(previewInstance);

        // 새로운 무기 생성
        previewInstance = Instantiate(weaponData.prefab, previewRoot);
        SetLayerRecursively(previewInstance, LayerMask.NameToLayer("UIWeapon"));

        // 무기 회전/스케일
        previewInstance.transform.localPosition = Vector3.zero;
        previewInstance.transform.localRotation = Quaternion.Euler(weaponData.previewRotation);
        previewInstance.transform.localScale = weaponData.previewScale;

        // 카메라 무기를 바라보게
        renderCamera.transform.LookAt(previewRoot.position);

        Debug.Log($"{gameObject.name} 슬롯에 {equippedWeapon.weaponName} 장착됨");
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
