using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab; // The bullet prefab
    [SerializeField] private Transform projectileShootPosition; //Where the bullet shoots from
    [SerializeField] private Transform rotationYPivot; //Rotation of the base
    [SerializeField] private LayerMask enemyLayer;
    
    [SerializeField] private float shootTime = 1f;
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    private float shootTimer = 0f;

    private Transform closestEnemy;
    private const string ENEMY_TAG = "Enemy";


    private void Update()
    {
        OverlapSphereDetectEnemy();
    }

    void OverlapSphereDetectEnemy()
    {
        Collider[] results = new Collider[30];
        var size = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, results, enemyLayer);

        float closestDistance = float.MaxValue;
        
        for (int i = 0; i < size; i++)
        {
            if (results[i].CompareTag(ENEMY_TAG))
            {
                float enemyDistance = Vector3.Distance(transform.position, results[i].transform.position);

                if (enemyDistance < closestDistance)
                {
                    closestDistance = enemyDistance;
                    closestEnemy = results[i].transform;
                }

                Debug.Log("Enemy detected with OverlapSphere: " + results[i].name);
            }
        }
        
        if (closestEnemy != null) 
        {
            Vector3 direction = closestEnemy.position - rotationYPivot.position;
            direction.y = 0; // This line ensures rotation is only around the Y axis
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            rotationYPivot.rotation = Quaternion.Slerp(rotationYPivot.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        if (closestEnemy != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, closestEnemy.position);  
        }
    }
}
