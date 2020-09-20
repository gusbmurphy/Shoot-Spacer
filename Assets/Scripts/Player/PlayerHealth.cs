using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
  [SerializeField] int startingHitPoints = 10;

  [Header("HUD")]
  [SerializeField] Slider healthBar;
  [SerializeField] GameManager gameManager;

  [Header("Cheats")]
  [SerializeField] bool godModeOn = false;

  private int hitPoints;

  void Start()
  {
    hitPoints = startingHitPoints;
    healthBar.maxValue = startingHitPoints;
    healthBar.value = startingHitPoints;
  }

  void IDamageable.TakeDamage(int damage)
  {
    if (godModeOn) return;

    hitPoints -= damage;
    healthBar.value = hitPoints;
    // TODO give visual hit feedback
    if (hitPoints <= 0) Die();
  }

  private void Die()
  {
    Destroy(gameObject);
    // TODO give visual destruction feedback
    gameManager.GameOver();
  }
}
