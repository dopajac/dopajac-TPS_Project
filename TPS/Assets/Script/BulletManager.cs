using System;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    private Rigidbody bulletRigidbody;
    [SerializeField]
    private float moveSpeed = 10f;

    private float destoryTime = 3f;
    void Start()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    [Obsolete("Obsolete")]
    void Update()
    {
        destoryTime -= Time.deltaTime;
        if (destoryTime <= 0)
        {
            DestoryBullet();
        }

        BulletMove();
    }

    [Obsolete("Obsolete")]
    private void BulletMove()
    {
        bulletRigidbody.velocity = transform.forward * moveSpeed;
    }

    private void DestoryBullet()
    { 
        Destroy(gameObject);
        destoryTime = 3f;
    }

    private void OnTriggerEnter(Collider other)
    {
        DestoryBullet();
    }
}
