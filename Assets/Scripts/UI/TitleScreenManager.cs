using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] Button playButton;
    void Start() { playButton.onClick.AddListener(() => SceneManager.LoadScene("Level1")); }
}
