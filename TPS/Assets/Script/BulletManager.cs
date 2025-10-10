using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float baseDamage = 30f; // 무기에서 세팅해줄 수도 있음

    private Rigidbody _rb;
    private float _t;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _t = 0f;
    }

    private void Update()
    {
        _t += Time.deltaTime;
        if (_t >= lifeTime) DestroyBullet();
    }

    private void FixedUpdate()
    {
        if (_rb) _rb.linearVelocity = transform.forward * moveSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet")||other.CompareTag("SpawnArea")) return; // 서로 무시

        // === HitBox 검사 ===
        DummyHitBox hitBox = other.GetComponent<DummyHitBox>();
        if (hitBox != null && hitBox.targetHealth != null)
        {
            float finalDamage = baseDamage * hitBox.damageMultiplier;
            hitBox.targetHealth.TakeDamage(finalDamage);

            Debug.Log($"총알이 {other.name} 에 명중! 최종데미지: {finalDamage}");
        }

        DestroyBullet();
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
}