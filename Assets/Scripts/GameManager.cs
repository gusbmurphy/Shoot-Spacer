using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Text gameOverText;
    [SerializeField] Text enemyCountText;
    private int numOfEnemies = 0;

    void Start()
    {
        print(SceneManager.sceneCountInBuildSettings);
        print("Build index: " + SceneManager.GetActiveScene().buildIndex);
        gameOverText.color = Color.clear;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        print(enemies);
        numOfEnemies = enemies.Length;
        enemyCountText.text = numOfEnemies + " ENEMIES LEFT";
    }
    public void GameOver()
    {
        gameOverText.color = Color.white;
    }

    public void EnemyDeath()
    {
        numOfEnemies--;
        enemyCountText.text = numOfEnemies + " ENEMIES LEFT";
        if (numOfEnemies < 1)
        {
            FinishLevel();
        }
    }

    public void FinishLevel()
    {
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        if (buildIndex + 1 >= SceneManager.sceneCountInBuildSettings)
        {
            print("All done!");
        } else
        {
            SceneManager.LoadScene(buildIndex + 1);
        }
    }
}