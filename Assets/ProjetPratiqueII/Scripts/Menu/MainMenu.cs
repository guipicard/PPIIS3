using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Slider m_ProgressBar;
    [SerializeField] private GameObject m_LoadScreen;

    private void Start()
    {
        m_LoadScreen.SetActive(false);
    }
    public void StartNewGame()
    {
        StartCoroutine(LoadGame());
    }

    private IEnumerator LoadGame()
    {
        AsyncOperation scene = SceneManager.LoadSceneAsync("Game");
        m_LoadScreen.SetActive(true);
        while (true)
        {
            if (scene.isDone)
            {
                SceneManager.UnloadSceneAsync("MainMenu");
            }
            else
            {
                m_ProgressBar.value = scene.progress;
                yield return null;
            }
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
