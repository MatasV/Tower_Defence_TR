using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerController : MonoBehaviour
{
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private Transform movePoint;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int numberOfEnemies = 10;

    private int spawnedCount = 0;

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (spawnedCount < numberOfEnemies)
        {
            yield return new WaitForSeconds(spawnInterval);

            Enemy enemy = Instantiate(enemyPrefab, transform.position, Quaternion.Euler(0, 180, 0));
            enemy.StartMovement(movePoint);
            
            spawnedCount++;
        }
    }
}
