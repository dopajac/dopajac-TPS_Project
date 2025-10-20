using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetRandomActivator : MonoBehaviour
{
    [Header("타겟 리스트 (Hierarchy에 있는 ShootingTarget_N 오브젝트들)")]
    [SerializeField] private List<GameObject> targets = new List<GameObject>();

    [Header("설정")]
    [SerializeField] private float activeTime = 2f;     // 타겟 유지 시간
    [SerializeField] private float delayBetween = 0.5f; // 다음 라운드 전 대기
    [SerializeField] private int targetsPerRound = 3;   // 라운드당 활성화 타겟 수
    [SerializeField] private int totalRounds = 15;      // 총 라운드 수

    private Coroutine spawnRoutine;

    private void Awake()
    {
        // ShootingTarget_N 자동 인식
        if (targets.Count == 0)
        {
            foreach (Transform child in transform)
            {
                if (child.name.StartsWith("ShootingTarget_N"))
                    targets.Add(child.gameObject);
            }
        }

        // 초기엔 전부 비활성화
        foreach (var t in targets)
            t.SetActive(false);
    }

    /// <summary>
    /// 버튼 등에서 호출해 시퀀스를 시작
    /// </summary>
    public void StartSequence()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
        }
        spawnRoutine = StartCoroutine(TargetSequence());
    }

    private IEnumerator TargetSequence()
    {
        WaitForSeconds activeWait = new WaitForSeconds(activeTime);
        WaitForSeconds delayWait = new WaitForSeconds(delayBetween);

        for (int round = 0; round < totalRounds; round++)
        {
            if (targets.Count == 0) yield break;

            // 랜덤 3개 선택
            List<GameObject> activeTargets = GetRandomTargets(targetsPerRound);

            // 선택된 타겟 활성화
            foreach (var t in activeTargets)
                t.SetActive(true);

            // 유지
            yield return activeWait;

            // 비활성화
            foreach (var t in activeTargets)
                t.SetActive(false);

            // 라운드 간 대기
            yield return delayWait;
        }

        // 🔹 종료 후 모든 타겟 ON
        foreach (var t in targets)
            t.SetActive(true);

        spawnRoutine = null;
        Debug.Log("🎯 TargetRandomActivator: 라운드 종료 후 모든 타겟 ON 상태로 복귀!");
    }

    private List<GameObject> GetRandomTargets(int count)
    {
        List<GameObject> selected = new List<GameObject>();
        List<GameObject> pool = new List<GameObject>(targets);

        int n = Mathf.Min(count, pool.Count);
        for (int i = 0; i < n; i++)
        {
            int index = Random.Range(0, pool.Count);
            selected.Add(pool[index]);
            pool.RemoveAt(index);
        }
        return selected;
    }

    /// <summary>
    /// 외부에서 강제로 정지하고 모든 타겟을 ON으로 바꾸고 싶을 때
    /// </summary>
    public void StopAndShowAll()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }

        foreach (var t in targets)
            t.SetActive(true);
    }
}
