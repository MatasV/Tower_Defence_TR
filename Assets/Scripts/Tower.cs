using System;
using System.Collections;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab; // The bullet prefab
    [SerializeField] private Transform projectileShootPosition; //Where the bullet shoots from
    [SerializeField] private Transform rotationYPivot; //Rotation of the base
    [SerializeField] private Transform rotationXpivot; //Rotation of the bow
    [SerializeField] private LayerMask enemyLayer;
    
    [SerializeField] private float shootTime = 1f;
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    
    private Transform closestEnemy;
    private const string ENEMY_TAG = "Enemy";

    private GameObject loadedProjectile;

    private void Start()
    {
        LoadProjectile();
    }

    private void Update()
    {
        OverlapSphereDetectEnemy();
    }

    void OverlapSphereDetectEnemy()
    {
        closestEnemy = null;
        Collider[] results = new Collider[30];
        var size = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, results, enemyLayer);

        float closestDistance = float.MaxValue;
        
        for (int i = 0; i < size; i++)
        {
            if (results[i].CompareTag(ENEMY_TAG))
            {
                float enemyDistance = Vector3.Distance(transform.position, results[i].transform.position);

                if (enemyDistance < closestDistance && results[i].gameObject.activeInHierarchy)
                {
                    closestDistance = enemyDistance;
                    closestEnemy = results[i].transform;
                }
            }
        }
        
        if (closestEnemy != null && closestEnemy.gameObject.activeInHierarchy) 
        {
            Vector3 directionToEnemy = closestEnemy.position - rotationYPivot.position;
        
            Vector3 yDirection = directionToEnemy;
            yDirection.y = 0;
            Quaternion yLookRotation = Quaternion.LookRotation(yDirection);
            rotationYPivot.rotation = Quaternion.Slerp(rotationYPivot.rotation, yLookRotation, Time.deltaTime * rotationSpeed);
        
            Vector3 relativeDirection = rotationYPivot.InverseTransformDirection(directionToEnemy);
            float xAngle = Mathf.Atan2(relativeDirection.y, relativeDirection.z) * Mathf.Rad2Deg;
            rotationXpivot.localEulerAngles = new Vector3(-xAngle, 0, 0);
        }
    }

    private void LoadProjectile()
    {
        loadedProjectile = Instantiate(projectilePrefab, projectileShootPosition.position, projectileShootPosition.rotation);
        loadedProjectile.transform.SetParent(rotationXpivot);
        loadedProjectile.transform.forward = rotationXpivot.transform.forward;
    }
    
    private void OnDrawGizmosSelected()
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
