using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    Rigidbody bulletRigidbody;

    [SerializeField] float moveSpeed = 10f;
    float destroyTime = 3f;

    // Start is called before the first frame update
    void Start()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        destroyTime -= Time.deltaTime;

        if (destroyTime <= 0)
        {
            DestroyBullet();
        }

        BulletMove();
    }

    void BulletMove()
    {
        bulletRigidbody.velocity = transform.forward * moveSpeed;
    }

    void DestroyBullet()
    {
        // Destroy(gameObject);
        gameObject.SetActive(false);
        destroyTime = 3;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<Enemy>().enemyCurrentHP -= 1;
        }

        DestroyBullet();
    }
}
