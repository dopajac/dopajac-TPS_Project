using System;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    private Rigidbody bulletRigidbody;
    [SerializeField]
    private float moveSpeed = 10f;
    void Start()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    [Obsolete("Obsolete")]
    void Update()
    {
        BulletMove();
    }

    [Obsolete("Obsolete")]
    private void BulletMove()
    {
        bulletRigidbody.velocity = transform.forward * moveSpeed;
    }
}
