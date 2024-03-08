using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileShootPosition;
    [SerializeField] private Transform rotationPivot;
    [SerializeField] private LayerMask enemyLayer;
    
    [SerializeField] private float shootTime = 1f;
    private float shootTimer = 0f;

    private Transform closestEnemy;
    private const string ENEMY_TAG = "Enemy";
    
    private void Update()
    {
        RaycastHit hit;
        float maxDistance = 0f;
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        if (Physics.SphereCast(origin, 100, direction, out hit, maxDistance))
        {
            if (hit.collider.CompareTag(ENEMY_TAG))
            {
                Debug.Log("DetectedEnemy");
            }
        }
    }
}
