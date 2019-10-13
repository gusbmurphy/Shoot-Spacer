using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] int startingHitPoints = 10;

    [Header("HUD")]
    [SerializeField] Slider healthBar;
    [SerializeField] Text gameOverText;

    private int hitPoints;

    void Start()
    {
        hitPoints = startingHitPoints;
        healthBar.maxValue = startingHitPoints;
        healthBar.value = startingHitPoints;
        gameOverText.color = Color.clear;
    }

    void IDamageable.TakeDamage(int damage)
    {
        hitPoints -= damage;
        healthBar.value = hitPoints;
        // TODO give visual hit feedback
        if (hitPoints <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Destroy(gameObject);
        // TODO give visual destruction feedback
        // TODO make a game end state
        gameOverText.color = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
