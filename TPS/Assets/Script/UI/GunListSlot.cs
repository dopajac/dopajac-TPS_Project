using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GunListSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("슬롯 UI")]
    public RawImage renderImage;   // 슬롯에 무기를 보여줄 RawImage

    private Camera renderCamera;   
    private GameObject previewInstance;
    private RenderTexture rt;
    private Transform previewRoot;

    private WeaponData currentWeapon; 

    // 드래그 전 위치/부모 저장
    private Transform originalParent;
    private Vector3 originalPosition;
    private Canvas rootCanvas;

    private void Awake()
    {
        rootCanvas = FindObjectOfType<Canvas>();
    }

    public void SetWeapon(WeaponData weaponData)
    {
        currentWeapon = weaponData;

        rt = new RenderTexture(512, 512, 16);
        renderImage.texture = rt;

        if (renderCamera == null)
        {
            GameObject camObj = new GameObject("RenderCam_" + weaponData.name);
            camObj.transform.SetParent(transform, false); 
            camObj.transform.localPosition = new Vector3(0, 0, -1f);

            renderCamera = camObj.AddComponent<Camera>();
            renderCamera.clearFlags = CameraClearFlags.SolidColor;
            renderCamera.backgroundColor = Color.clear;
            renderCamera.orthographic = false;
            renderCamera.cullingMask = LayerMask.GetMask("UIWeapon");
            renderCamera.targetTexture = rt;
        }

        if (previewRoot == null)
        {
            GameObject rootObj = new GameObject("PreviewRoot_" + weaponData.name);
            rootObj.transform.SetParent(transform, false);
            previewRoot = rootObj.transform;
        }

        if (previewInstance != null) Destroy(previewInstance);

        previewInstance = Instantiate(weaponData.prefab, previewRoot);
        SetLayerRecursively(previewInstance, LayerMask.NameToLayer("UIWeapon"));

        previewInstance.transform.localPosition = Vector3.zero;
        previewInstance.transform.localRotation = Quaternion.Euler(weaponData.previewRotation);
        previewInstance.transform.localScale = weaponData.previewScale;

        renderCamera.transform.LookAt(previewRoot.position);
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    // ================= 드래그 기능 =================
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentWeapon == null) return;

        DragDropManager.draggedWeapon = currentWeapon;

        // 원래 Preview 복사
        GameObject previewCopy = Instantiate(renderImage.gameObject, rootCanvas.transform);
        previewCopy.name = "DraggedPreview_" + currentWeapon.weaponName;

        // 위치/스케일 조정
        var rt = previewCopy.GetComponent<RectTransform>();
        rt.sizeDelta = renderImage.rectTransform.sizeDelta;
        rt.position = eventData.position;

        // 드래그 중 미리보기 등록
        DragDropManager.draggedPreview = previewCopy.GetComponent<RawImage>();

        // 복사본은 드래그 중에만 보여야 하므로 RaycastTarget 꺼둠
        DragDropManager.draggedPreview.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (DragDropManager.draggedPreview != null)
        {
            DragDropManager.draggedPreview.rectTransform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드랍이 실패했으면 삭제
        if (DragDropManager.draggedPreview != null)
        {
            Destroy(DragDropManager.draggedPreview.gameObject);
            DragDropManager.draggedPreview = null;
        }

        DragDropManager.draggedWeapon = null;
    }
}
