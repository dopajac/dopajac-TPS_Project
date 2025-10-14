using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private List<GameObject> spawnPlayersPoint;
    [SerializeField] private Transform moveTarget;  // 이동할 실제 타깃 (PlayerArmature 등)
    [SerializeField] private Rigidbody rb;

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
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = spawnPos;
        }
        else
        {
            moveTarget.position = spawnPos;
        }

        Debug.Log($"[SpawnManager] 이동 완료 → Room: {RoomNumber}");
    }
}