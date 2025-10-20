using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class DummyNav : MonoBehaviour
{
    [SerializeField] private Transform targetPosition1;
    [SerializeField] private Transform targetPosition2;

    private NavMeshAgent agent;
    private Animator animator;   // Animator 추가
    private Vector3 startPosition;

    private int currentIndex = 0;  // 0 = start, 1 = target1, 2 = target2
    private bool isWaiting = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();   // Animator 컴포넌트 가져오기
        startPosition = transform.position;    // 시작 위치 저장
    }

    void Start()
    {
        // 첫 목적지는 target1
        currentIndex = 1;
        agent.SetDestination(targetPosition1.position);

        if (animator != null) animator.SetBool("Stop", false); // 이동 시작
    }

    void Update()
    {
        if (agent.pathPending || isWaiting) return;

        // 도착 확인
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(WaitAndGoNext());
        }
    }

    private IEnumerator WaitAndGoNext()
    {
        isWaiting = true;

        // 도착 → 애니메이션 정지
        if (animator != null) animator.SetBool("Stop", true);

        yield return new WaitForSeconds(1f);

        // 다음 목적지로 이동 시작
        if (currentIndex == 1) // target1 → target2
        {
            agent.SetDestination(targetPosition2.position);
            currentIndex = 2;
        }
        else if (currentIndex == 2) // target2 → start
        {
            agent.SetDestination(startPosition);
            currentIndex = 0;
        }
        else if (currentIndex == 0) // start → target1
        {
            agent.SetDestination(targetPosition1.position);
            currentIndex = 1;
        }

        // 이동 시작 → 애니메이션 재생
        if (animator != null) animator.SetBool("Stop", false);

        isWaiting = false;
    }
    public void RestartMovement()
    {
        isWaiting = false;
        currentIndex = 1; // 다시 target1으로 시작
        if (agent != null && targetPosition1 != null)
        {
            agent.SetDestination(targetPosition1.position);
        }

        if (animator != null)
            animator.SetBool("Stop", false);
    }
}