using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
  private int numOfWaves, numOfCompletedWaves = 0;
  public Text gameOverText;
  public Text levelCompleteText;
  private int buildIndex;

  void Start()
  {
    buildIndex = SceneManager.GetActiveScene().buildIndex;

    gameOverText.color = Color.clear;
    levelCompleteText.color = Color.clear;

    EnemyWave[] waves = FindObjectsOfType<EnemyWave>();
    numOfWaves = waves.Length;

    void WaveCompletionHandler()
    {
      numOfCompletedWaves++;
      if (numOfCompletedWaves >= numOfWaves) FinishLevel();
    }

    foreach (EnemyWave wave in waves) wave.OnCompletion += WaveCompletionHandler;
  }
  public void GameOver()
  {
    gameOverText.color = Color.white;
  }

  private IEnumerator LevelOverScreenTimeout()
  {
    yield return new WaitForSeconds(3);
    SceneManager.LoadScene(buildIndex + 1);
  }
  public void FinishLevel()
  {
    if (buildIndex + 1 >= SceneManager.sceneCountInBuildSettings)
    {
      print("?");
      throw new NotImplementedException("The game is over!");
    }
    else
    {
      levelCompleteText.color = Color.white;
      StartCoroutine(LevelOverScreenTimeout());
    }
  }
}
