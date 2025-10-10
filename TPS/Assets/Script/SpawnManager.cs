using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> spawnPlayersPoint;
    [SerializeField] private int roomnumber;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); // Player에 붙은 Rigidbody 가져오기
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SetPlayerSpawn(roomnumber);
        }
    }

    public void SetPlayerSpawn(int RoomNumber)
    {
        if (spawnPlayersPoint == null || spawnPlayersPoint.Count <= RoomNumber)
        {
            Debug.LogWarning("SpawnManager: 해당 RoomNumber의 스폰포인트가 없음!");
            return;
        }

        Vector3 spawnPos = spawnPlayersPoint[RoomNumber].transform.position;

        if (rb != null)
        {
            // ✅ Rigidbody가 있을 때는 MovePosition으로 이동
            rb.MovePosition(spawnPos);
        }
        else
        {
            // Rigidbody 없으면 일반 이동
            transform.position = spawnPos;
        }

        Debug.Log($"플레이어 이동 완료 → Room: {RoomNumber}, 위치: {spawnPos}");
    }
}
