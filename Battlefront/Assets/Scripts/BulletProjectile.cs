using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    private Rigidbody rb;

    private float lifetime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        float speed = 40f;
        rb.velocity = transform.forward * speed;

        lifetime = 10f;
    }

    private void Update()
    {
        lifetime -= Time.deltaTime;

        if(lifetime < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Bullet") { return; }
        Destroy(gameObject);
    }
}
