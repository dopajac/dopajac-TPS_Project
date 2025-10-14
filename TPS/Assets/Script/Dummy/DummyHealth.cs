using UnityEngine;

public class DummyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
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

    void Die()
    {
        Debug.Log($"{gameObject.name} 사망!");
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddDummyScore(1);
            Debug.Log($"Dummy Score +{1}! 현재 총 점수: {ScoreManager.Instance.Dummy_Score}");
        }
        else
        {
            Debug.LogWarning("ScoreManager 인스턴스를 찾을 수 없습니다!");
        }
        Destroy(gameObject);
        
        // 죽는 애니메이션 or 리스폰 처리
    }
}
