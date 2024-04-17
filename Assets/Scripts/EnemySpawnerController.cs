using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerController : MonoBehaviour
{
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private Transform movePoint;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private WaveManager waveManager;

    private int spawnedCount = 0;

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while(waveManager.currentWave < waveManager.waveConfiguration.WaveCount.Length) //tikriname ar dabartinis wave'as musu wave'u masyvo ribose
        {
            int numberOfEnemies = waveManager.GetNumberOfEnemiesInCurrentWave(); //gauname is SO kiek enemies turetu buti esamame wave
            for(int i = 0; i < numberOfEnemies; i++)
            {
                if(!waveManager.IsPaused()) //jei spawninimas neuzpausintas instantiatinam enemy prefab'us
                {
                    Enemy enemy = Instantiate(enemyPrefab, transform.position, Quaternion.Euler(0, 180, 0));
                    enemy.StartMovement(movePoint);

                    spawnedCount++;
                }
                else
                {
                    yield return new WaitUntil(() => !waveManager.IsPaused());
                }
                yield return new WaitForSeconds(spawnInterval);
            }
            waveManager.IncrementWave();
            yield return new WaitForSeconds(waveManager.pauseDuration);
        }

    }
}
