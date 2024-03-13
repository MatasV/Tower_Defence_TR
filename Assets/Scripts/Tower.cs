using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Tower : MonoBehaviour
{
    [SerializeField] private Projectile projectilePrefab; // The bullet prefab
    [SerializeField] private Transform projectileShootPosition; //Where the bullet shoots from
    [SerializeField] private Transform rotationYPivot; //Rotation of the base
    [SerializeField] private Transform rotationXPivot; //Rotation of the bow
    [SerializeField] private LayerMask enemyLayer;

    [SerializeField] private float shootTime = 1f;
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    [SerializeField] private float viewAngle = 45f;

    private Enemy closestEnemy;
    private const string ENEMY_TAG = "Enemy";

    private Projectile loadedProjectile;

    private bool isShooting = false;

    public enum TowerState
    {
        Idle,
        Attacking
    }

    private TowerState previousState = TowerState.Attacking;
    public TowerState towerState = TowerState.Idle;

    private float lastRotationY = float.PositiveInfinity;

    private void Start()
    {
        LoadProjectile();
    }


    private void Update()
    {
        OverlapSphereDetectEnemy();

        switch (towerState)
        {
            case TowerState.Idle:
                if (previousState == TowerState.Attacking)
                {
                    ScanForEnemies();
                }

                if (CheckEnemyToShoot(closestEnemy))
                {
                    towerState = TowerState.Attacking;
                }

                break;

            case TowerState.Attacking:
                if (!CheckEnemyToShoot(closestEnemy))
                {
                    towerState = TowerState.Idle;
                    LeanTween.cancel(rotationYPivot.gameObject);
                    LeanTween.cancel(rotationXPivot.gameObject);
                    ScanForEnemies();
                }
                else
                {
                    LeanTween.cancel(rotationYPivot.gameObject);
                    LeanTween.cancel(rotationXPivot.gameObject);
                    if (!isShooting)
                    {
                        StartCoroutine(ShootAnEnemy());
                    }
                }

                break;
        }

        previousState = towerState;
    }

    private void ScanForEnemies()
    {
        if (float.IsPositiveInfinity(lastRotationY))
        {
            lastRotationY = rotationYPivot.rotation.eulerAngles.y;
        }

        float newYRotation = UnityEngine.Random.Range(lastRotationY - 45f, lastRotationY + 45f);
        newYRotation = newYRotation < 0 ? newYRotation + 360 : newYRotation;
        newYRotation = newYRotation > 360 ? newYRotation - 360 : newYRotation;
        float newXRotation = UnityEngine.Random.Range(-15f, 30f);

        LeanTween.cancel(rotationYPivot.gameObject);
        LeanTween.cancel(rotationXPivot.gameObject);

        LeanTween.rotateY(rotationYPivot.gameObject, newYRotation, 2f).setDelay(2f)
            .setEase(LeanTweenType.easeInOutQuad);
        LeanTween.rotateX(rotationXPivot.gameObject, newXRotation, 2f).setDelay(2f).setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(ScanForEnemies);

        lastRotationY = newYRotation;
    }

    private IEnumerator ShootAnEnemy()
    {
        if (isShooting)
        {
            yield break;
        }

        isShooting = true;

        while (CheckEnemyForTarget(closestEnemy))
        {
            if (loadedProjectile == null || loadedProjectile.isFlying)
            {
                LoadProjectile();
            }

            loadedProjectile.Init(closestEnemy.transform);
            yield return new WaitForSeconds(shootTime);

            if (!CheckEnemyForTarget(closestEnemy))
            {
                LoadProjectile();
            }
        }

        isShooting = false;
    }

    private void OverlapSphereDetectEnemy()
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

                Enemy enemy = results[i].GetComponent<Enemy>();
                if (enemy == null)
                {
                    continue;
                }

                if (enemyDistance < closestDistance && CheckEnemyForTarget(enemy))
                {
                    closestDistance = enemyDistance;
                    closestEnemy = results[i].GetComponent<Enemy>();
                }
            }
        }

        //rotation
        if (CheckEnemyForTarget(closestEnemy))
        {
            Vector3 directionToEnemy = closestEnemy.transform.position - rotationYPivot.position;

            Vector3 yDirection = directionToEnemy;
            yDirection.y = 0;
            Quaternion yLookRotation = Quaternion.LookRotation(yDirection);
            rotationYPivot.rotation =
                Quaternion.Slerp(rotationYPivot.rotation, yLookRotation, Time.deltaTime * rotationSpeed);

            Vector3 relativeDirection = rotationYPivot.InverseTransformDirection(directionToEnemy);
            float xAngle = Mathf.Atan2(relativeDirection.y, relativeDirection.z) * Mathf.Rad2Deg;
            rotationXPivot.localEulerAngles = new Vector3(-xAngle, 0, 0);
        }
    }

    private bool CheckEnemyForTarget(Enemy enemy)
    {
        if (enemy != null && enemy.gameObject.activeInHierarchy && !enemy.isDead)
        {
            return true;
        }

        return false;
    }

    private bool CheckEnemyToShoot(Enemy enemy)
    {
        if (CheckEnemyForTarget(enemy))
        {
            Vector3 directionToEnemy = (enemy.transform.position - rotationYPivot.position);
            directionToEnemy.y = 0; // Ignore the y-axis (height)
            directionToEnemy = directionToEnemy.normalized;
            float dotProduct = Vector3.Dot(rotationYPivot.forward, directionToEnemy);
            if (dotProduct > Mathf.Cos(viewAngle * 0.5f * Mathf.Deg2Rad))
            {
                return true;
            }
        }

        return false;
    }

    private void LoadProjectile()
    {
        loadedProjectile = Instantiate(projectilePrefab, projectileShootPosition.position,
            projectileShootPosition.rotation);
        loadedProjectile.transform.SetParent(rotationXPivot);
        loadedProjectile.transform.forward = rotationXPivot.transform.forward;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, rotationYPivot.forward * detectionRadius);

        Gizmos.color = Color.cyan;
        DrawViewAngleGizmo();

        if (closestEnemy != null && closestEnemy.gameObject.activeInHierarchy)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, closestEnemy.transform.position);
        }
    }

    private void DrawViewAngleGizmo()
    {
        int segments = 50;
        float stepAngleSize = viewAngle / segments;
        Vector3 previousPoint = rotationYPivot.forward;

        for (int i = 0; i <= segments; i++)
        {
            Vector3 direction = Quaternion.Euler(0, -viewAngle / 2 + stepAngleSize * i, 0) * rotationYPivot.forward;
            Vector3 endPoint = transform.position + direction.normalized * detectionRadius;

            if (i > 0)
            {
                Gizmos.DrawLine(previousPoint, endPoint);
            }

            Gizmos.DrawLine(transform.position, endPoint);
            Gizmos.DrawLine(endPoint, endPoint - direction.normalized * (detectionRadius / segments));

            previousPoint = endPoint;
        }
    }
}