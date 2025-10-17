using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GunInventorySlot : MonoBehaviour, IDropHandler
{
    [Header("UI 연결")]
    public int slotIndex; // 0 = 무기1, 1 = 무기2

    [SerializeField] private AttachmentPanelController attachmentPanel; // 슬롯별 패널 연결
    
    
    private Camera renderCamera;
    private GameObject previewInstance;
    private RenderTexture rt;
    private Transform previewRoot;

    private WeaponData equippedWeapon;
    private WeaponHolder playerWeaponHolder;

    private void Awake()
    {
        // 플레이어의 WeaponHolder 찾기 (씬 안에 하나 있다고 가정)
        playerWeaponHolder = FindObjectOfType<WeaponHolder>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (DragDropManager.draggedWeapon != null && DragDropManager.draggedPreview != null)
        {
            // 기존 프리뷰 삭제
            if (previewInstance != null) Destroy(previewInstance);

            // 드래그된 프리뷰를 슬롯에 붙임
            previewInstance = DragDropManager.draggedPreview.gameObject;
            previewInstance.transform.SetParent(transform, false);
            previewInstance.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            equippedWeapon = DragDropManager.draggedWeapon;
            Debug.Log($"{gameObject.name} 슬롯에 {equippedWeapon.weaponType} 장착됨 (UI)");

            EquipToPlayer(equippedWeapon);

            DragDropManager.draggedPreview = null;
            DragDropManager.draggedWeapon = null;
        }
    }


    private void EquipToPlayer(WeaponData weaponData)
    {
        if (!playerWeaponHolder) return;

        // ✅ weaponData를 직접 넘겨서 타입으로 탐색
        Weapon found = FindWeaponInScene(weaponData);
        if (found == null)
        {
            Debug.LogError($"씬 안에서 {weaponData.weaponName} 오브젝트를 찾을 수 없음!");
            return;
        }

        Weapon old = playerWeaponHolder.GetWeapon(slotIndex);
        if (old) old.gameObject.SetActive(false);

        playerWeaponHolder.SetWeapon(slotIndex, found);
        Debug.Log($"플레이어 {slotIndex}번 슬롯에 {weaponData.weaponName} 연결됨 (씬 오브젝트)");
    }

// Scene 내 무기 찾기 (이름으로)
    private Weapon FindWeaponInScene(WeaponData weaponData)
    {
        Weapon[] allWeapons = FindObjectsOfType<Weapon>(true);
        foreach (var w in allWeapons)
        {
            // UIWeapon Layer 제외
            if (w.gameObject.layer == LayerMask.NameToLayer("UIWeapon"))
                continue;

            // ✅ weaponType 비교 (정확)
            if (w.weaponType == weaponData.weaponType)
                return w;
        }
        return null;
    }

    public void SetWeapon(WeaponData weaponData)
    {
        equippedWeapon = weaponData;

        // 기존 프리뷰 제거
        if (previewInstance != null)
        {
            Destroy(previewInstance);
            previewInstance = null;
        }
        // RenderTexture 새로 생성
        rt = new RenderTexture(512, 512, 16);

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

        Debug.Log($"{gameObject.name} 슬롯 UI에 {equippedWeapon.weaponName} 프리뷰 생성됨");
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
