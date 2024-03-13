using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
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
