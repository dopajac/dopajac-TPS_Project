using UnityEngine;

public class DummyHitBox : MonoBehaviour
{
    public float damageMultiplier = 1f; // 머리 2f, 팔/다리 0.7f, 몸통/골반 1f
    public DummyHealth targetHealth;        // 캐릭터의 Health 참조
}
