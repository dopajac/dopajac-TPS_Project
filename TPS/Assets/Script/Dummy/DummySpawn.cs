using System.Collections;
using UnityEngine;

public class DummySpawn : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] spawnPrefabs; // 여러 개의 프리팹 목록
    [SerializeField] private float spawnInterval = 1f;  // 스폰 주기 (초)
    [SerializeField] private int maxSpawnCount = 10;    // 최대 스폰 개수
    [SerializeField] private float fixedY = 0.12f;      // Y 고정값

    private BoxCollider boxCollider;
    private int currentCount = 0;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    public void Spawn()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (currentCount < maxSpawnCount)
        {
            SpawnRandom();
            currentCount++;
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnRandom()
    {
        if (spawnPrefabs == null || spawnPrefabs.Length == 0)
        {
            Debug.LogWarning("SpawnArea: 스폰할 프리팹이 없습니다!");
            return;
        }

        // 실제 월드 크기 계산
        Vector3 worldSize = Vector3.Scale(boxCollider.size, transform.lossyScale);
        Vector3 worldCenter = transform.TransformPoint(boxCollider.center);

        float x = Random.Range(-worldSize.x / 2, worldSize.x / 2);
        float z = Random.Range(-worldSize.z / 2, worldSize.z / 2);

        // Y는 고정
        Vector3 localPos = new Vector3(x, fixedY, z);
        Vector3 spawnPos = worldCenter + transform.rotation * localPos;

        // 랜덤 프리팹 선택
        GameObject prefabToSpawn = spawnPrefabs[Random.Range(0, spawnPrefabs.Length)];

        // 뒤돌아보게 회전
        Quaternion rot = Quaternion.Euler(0, 180f, 0);

        Instantiate(prefabToSpawn, spawnPos, rot);
    }

    private void OnDrawGizmosSelected()
    {
        if (!GetComponent<BoxCollider>()) return;
        Gizmos.color = Color.green;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(GetComponent<BoxCollider>().center, GetComponent<BoxCollider>().size);
    }
}
