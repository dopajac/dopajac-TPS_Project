using UnityEngine;
using System.Collections;

public class DummyRespawnHelper : MonoBehaviour
{
    public static DummyRespawnHelper Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Respawn(DummyHealth dummy)
    {
        // 비활성화 되기 전에 Coroutine 실행
        StartCoroutine(RespawnRoutine(dummy));
    }

    private IEnumerator RespawnRoutine(DummyHealth dummy)
    {
        GameObject dummyObj = dummy.gameObject;
        dummyObj.SetActive(false);

        yield return new WaitForSeconds(1f); // 1초 대기

        dummyObj.SetActive(true);
        dummy.Respawn(); // 부활 처리 호출
    }
}