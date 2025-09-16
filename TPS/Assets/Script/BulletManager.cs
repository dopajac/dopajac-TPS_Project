using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float lifeTime = 3f;

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
        DestroyBullet();
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
}