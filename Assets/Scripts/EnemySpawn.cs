using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
  [SerializeField] GameObject enemyToSpawn;
  private bool _hasSpawned = false;
  public bool HasSpawned { get => _hasSpawned; }
  private bool _enemyDefeated = false;
  public bool EnemyDefeated { get => _enemyDefeated; }
  private GameObject spawnedEnemy;
  public event Action OnCompletion;

  private void OnDrawGizmos()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawSphere(transform.position, 1);
  }
  public void Spawn()
  {
    if (_hasSpawned)
    {
      Debug.LogException(new UnityException("Spawner has already spawned!"));
      return;
    }
    spawnedEnemy = Instantiate(enemyToSpawn, transform);
    _hasSpawned = true;

    void DeathHandler() { 
      _enemyDefeated = true;
      if (OnCompletion != null) OnCompletion();
    }
    spawnedEnemy.GetComponent<Enemy>().OnDeath += DeathHandler;
  }
}
