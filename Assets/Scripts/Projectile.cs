using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool isFlying = false;

    [SerializeField] private float speed;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float destroyTime = 5f;
    [SerializeField] private int damage;
    public void Init(Transform target)
    {
        isFlying = true;
        
        transform.SetParent(null);
        
        Vector3 direction = target.position - transform.position;
        transform.forward = direction;
        
        rb.AddForce(transform.forward * speed, ForceMode.Impulse);
        Destroy(gameObject, destroyTime);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.GetHit(damage);
            }
            else
            {
                Debug.LogError("No Enemy script found!");
            }
            
            Destroy(gameObject);
        }
    }
}
