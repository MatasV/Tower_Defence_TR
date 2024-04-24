using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public WaveConfiguration waveConfiguration;
    public int currentWave = 0;
    public int pauseDuration = 5;

    private bool isPaused = false;

    public int GetNumberOfEnemiesInCurrentWave()
    {
        if(currentWave < waveConfiguration.WaveCount.Length)
        {
            return waveConfiguration.WaveCount[currentWave];
        }
        else
        {
            return 0;
        }
    }

    public void IncrementWave()
    {
        currentWave++;
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}
