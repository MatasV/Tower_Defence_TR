using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform targetTransform;
    public float speed = 5f;
    public float distanceToDestroy = 0.1f;
    public int damage = 15;
    public bool isDead = false;
    public int health;
    [SerializeField] private Animator animator;

    public void GetHit(int damage)
    {
        if (isDead)
        {
            return;
        }
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    public void StartMovement(Transform target)
    {
        targetTransform = target;
        animator.SetBool("Run", true);
    }

    private void Update()
    {
        MoveTowardsTarget();
        CheckDistanceToTarget();
    }

    private void CheckDistanceToTarget()
    {
        if (Vector3.Distance(transform.position, targetTransform.position) < distanceToDestroy)
        {
            Destroy(gameObject);
            if (HealthController.Instance != null)
            {
                HealthController.Instance.DoDamageToPlayer(damage);
            }
        }
    }

    private void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, speed * Time.deltaTime);
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        animator.SetBool("Death", true);
        Destroy(gameObject, 3f); //todo get the required wait time from death animation
    }
}
