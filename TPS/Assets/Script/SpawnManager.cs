using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private List<GameObject> spawnPlayersPoint;
    [SerializeField] private Transform moveTarget;  // 이동할 실제 타깃 (PlayerArmature 등)
    [SerializeField] private Rigidbody rb;

    [Header("UI 텍스트")]
    [SerializeField] private GameObject dummyScoreText;
    [SerializeField] private GameObject targetScoreText;
    
    private void Awake()
    {
        if (!moveTarget) moveTarget = transform;
        if (!rb) rb = GetComponent<Rigidbody>();
    }

    public void SetPlayerSpawn(int RoomNumber)
    {
        if (spawnPlayersPoint == null || spawnPlayersPoint.Count <= RoomNumber)
        {
            Debug.LogWarning("SpawnManager: 해당 RoomNumber의 스폰포인트가 없음!");
            return;
        }

        Vector3 spawnPos = spawnPlayersPoint[RoomNumber].transform.position + Vector3.up * 0.05f;
        Debug.Log($"[SpawnManager] 이동 시도 → {spawnPos}");

        var cc = moveTarget.GetComponent<CharacterController>();
        if (cc)
        {
            cc.enabled = false;
            moveTarget.position = spawnPos;
            cc.enabled = true;
        }
        else if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = spawnPos;
        }
        else
        {
            moveTarget.position = spawnPos;
        }

        // ✅ 점수 텍스트 활성화 로직
        if (RoomNumber == 0 || RoomNumber == 1 || RoomNumber == 4)
        {
            dummyScoreText.SetActive(true);
            targetScoreText.SetActive(false);
            Debug.Log("Dummy Score UI 활성화");
        }
        else if (RoomNumber == 2 || RoomNumber == 3)
        {
            dummyScoreText.SetActive(false);
            targetScoreText.SetActive(true);
            Debug.Log("Target Score UI 활성화");
        }
        else
        
        {
            // 예외 처리 (모두 끄기)
            dummyScoreText.SetActive(false);
            targetScoreText.SetActive(false);
        }

        Debug.Log($"[SpawnManager] 이동 완료 → Room: {RoomNumber}");
    }
}