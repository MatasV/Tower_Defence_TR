using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Transform targetTransform;
    public float speed = 5f;
    public float distanceToDestroy = 0.1f;
    public int damage = 15;
    public bool isDead = false;
    public int health;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    public void GetHit(int damage)
    {
        if (isDead)
        {
            return;
        }
        health -= damage;
        if (health <= 0f)
        {
            Die();
        }
    }

    public void StartMovement(Transform target)
    {
        targetTransform = target;
        animator.SetBool("Run", true);
        agent.SetDestination(targetTransform.position);
    }

    private void Update()
    {
        if (targetTransform == null)
        {
            return;
        }
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
    
    private void Die()
    {
        if (isDead)
        {
            return;
        }

        EnemySpawnerController.Instance.EnemyDied();
        
        agent.enabled = false;
        
        isDead = true;
        animator.SetBool("Death", true);
        Destroy(gameObject, 3f); //todo get the required wait time from death animation
    }
}
