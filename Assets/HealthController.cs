using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    public static HealthController Instance;
    [SerializeField] private int startingHP = 100;
    [SerializeField] private Slider healthSlider;
    private int health;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        health = startingHP;
        healthSlider.maxValue = health;
        healthSlider.value = health;
    }

    public void DoDamageToPlayer(int damage)
    {
        health -= damage;
        healthSlider.value = health;
        if (health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
