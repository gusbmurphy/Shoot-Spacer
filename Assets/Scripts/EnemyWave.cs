using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWave : MonoBehaviour
{
  public int secondsToBegin = 0;
  private bool _isCompleted = false;
  public bool IsCompleted { get => _isCompleted; }
  public event Action OnCompletion;
  private int numOfSpawns;
  private int numOfDefeatedSpawns = 0;
  EnemySpawn[] spawns;
  
  void Start()
  {
    StartCoroutine(BeginCountdown());
    spawns = GetComponentsInChildren<EnemySpawn>();
    numOfSpawns = spawns.Length;

    void SpawnCompletionHandler()
    {
      numOfDefeatedSpawns++;
      if (numOfDefeatedSpawns >= numOfSpawns && OnCompletion != null) OnCompletion();
    }
    foreach (EnemySpawn spawn in spawns) spawn.OnCompletion += SpawnCompletionHandler;
  }

  private IEnumerator BeginCountdown()
  {
    yield return new WaitForSeconds(secondsToBegin);
    StartWave();
  }

  private void StartWave()
  {
    foreach (EnemySpawn spawn in spawns) spawn.Spawn();
  }
}

