using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private List<GameObject> spawnPlayersPoint;
    [SerializeField] private Transform moveTarget;  // 실제 이동할 플레이어 오브젝트 (PlayerArmature 등)

    [Header("UI 텍스트")]
    [SerializeField] private GameObject dummyScoreText;
    [SerializeField] private GameObject targetScoreText;

    [SerializeField] private GameObject SpawnPanel;
    private CharacterController cc;

    [SerializeField] private StarterAssets.StarterAssetsInputs input; // StarterAssetsInputs 연결
    private void Awake()
    {
        if (!moveTarget)
            moveTarget = transform;

        // 플레이어에 CharacterController가 붙어있다면 캐싱
        cc = moveTarget.GetComponent<CharacterController>();
        if (!cc)
            Debug.LogWarning("[SpawnManager] CharacterController를 찾을 수 없습니다!");
    }

    public void SetPlayerSpawn(int RoomNumber)
    {
        if (spawnPlayersPoint == null || spawnPlayersPoint.Count <= RoomNumber)
        {
            Debug.LogWarning("[SpawnManager] 해당 RoomNumber의 스폰포인트가 없음!");
            return;
        }

        Vector3 spawnPos = spawnPlayersPoint[RoomNumber].transform.position + Vector3.up * 0.05f;
        Debug.Log($"[SpawnManager] 이동 시도 → {spawnPos}");

        // ✅ CharacterController를 통한 위치 이동
        if (cc != null)
        {
            cc.enabled = false;                // 순간이동 전 disable
            moveTarget.position = spawnPos;    // 위치 직접 세팅
            cc.enabled = true;                 // 다시 enable
        }
        else
        {
            moveTarget.position = spawnPos;    // fallback
        }

        // ✅ 점수 텍스트 활성화 로직
        switch (RoomNumber)
        {
            case 0:
            case 1:
            case 4:
                dummyScoreText.SetActive(true);
                targetScoreText.SetActive(false);
                Debug.Log("Dummy Score UI 활성화");
                break;

            case 2:
            case 3:
                dummyScoreText.SetActive(false);
                targetScoreText.SetActive(true);
                Debug.Log("Target Score UI 활성화");
                break;

            default:
                dummyScoreText.SetActive(false);
                targetScoreText.SetActive(false);
                Debug.Log("UI 비활성화 (해당되지 않음)");
                break;
        }

        Debug.Log($"[SpawnManager] 이동 완료 → Room: {RoomNumber}");
        SpawnPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;  
        Cursor.visible = false;   
        if (input) input.cursorInputForLook = true;
    }
}
