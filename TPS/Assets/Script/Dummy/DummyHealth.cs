using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class DummyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    private NavMeshAgent agent;   // 있을 수도, 없을 수도 있음
    private DummyNav dummyNav;    // 있을 수도 있음

    private void Awake()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        dummyNav = GetComponent<DummyNav>();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} 피격! 남은 체력: {currentHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} 사망!");

        // 점수 추가
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddDummyScore(1);
        }

        // NavMeshAgent가 있을 때만 이동 정지
        if (agent != null)
            agent.isStopped = true;

        // 비활성화 및 리스폰 관리
        DummyRespawnHelper.Instance.Respawn(this);
    }

    public void Respawn()
    {
        gameObject.SetActive(true);
        currentHealth = maxHealth;

        // NavMeshAgent가 있을 경우만 속도 2배
        if (agent != null)
        {
            agent.isStopped = false;
            agent.speed *= 2f;
        }

        // DummyNav가 있을 경우에만 이동 재시작
        if (dummyNav != null)
        {
            dummyNav.RestartMovement();
        }

        Debug.Log($"{gameObject.name} 부활 완료! (NavMeshAgent 존재: {agent != null})");
    }
}