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
  public event Action<Transform> OnSpawnedDeath;
  public event Action<Transform> OnEnemySpawn;

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
    if (OnEnemySpawn != null) OnEnemySpawn.Invoke(spawnedEnemy.transform);

    void DeathHandler()
    {
      _enemyDefeated = true;
      if (OnSpawnedDeath != null) OnSpawnedDeath(spawnedEnemy.transform);
    }
    spawnedEnemy.GetComponent<Enemy>().OnDeath += DeathHandler;
  }
}
