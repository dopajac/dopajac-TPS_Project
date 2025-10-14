using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; } // 싱글톤 인스턴스

    public int Dummy_Score = 0;
    public int Target_Score = 0;

    [SerializeField] private TextMeshProUGUI Dummy_Score_text;
    [SerializeField] private TextMeshProUGUI Target_Score_text;

    private void Awake()
    {
        // 싱글톤 초기화
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지되게 함 (필요 없으면 제거 가능)
    }

    private void Update()
    {
        Dummy_Score_text.text = "Dummy Score: " + Dummy_Score;
        Target_Score_text.text = "Target Score: " + Target_Score;
    }

    // 점수 조작용 메서드 예시
    public void AddDummyScore(int value)
    {
        Dummy_Score += value;
    }

    public void AddTargetScore(int value)
    {
        Target_Score += value;
    }

    public void ResetScores()
    {
        Dummy_Score = 0;
        Target_Score = 0;
    }
}