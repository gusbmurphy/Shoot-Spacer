using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWave : MonoBehaviour
{
    [SerializeField] int secondsToBegin = 0;
    EnemySpawn[] spawns;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BeginCountdown());
        spawns = GetComponentsInChildren<EnemySpawn>();
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
  
