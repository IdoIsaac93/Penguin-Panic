using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Singelton
    public static GameManager Instance { get; private set; }

    //Globally accesible player reference
    [SerializeField] private GameObject player;
    public GameObject Player => player;

    //UI
    [SerializeField] private GameObject loseScreen;
    [SerializeField] private GameObject WinScreen;
    [SerializeField] private GameObject pauseMenu;
    private void Awake()
    {
        //Singelton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            //Subscribe to events
            SceneManager.sceneLoaded += OnSceneLoaded;
            PlayerHealth.OnPlayerDeath += TriggerLoseScreen;
            LevelManager.OnLevelComplete += TriggerWinScreen;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        PlayerHealth.OnPlayerDeath -= TriggerLoseScreen;
        LevelManager.OnLevelComplete -= TriggerWinScreen;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindWithTag("Player");
    }

    //Win Screen
    public void TriggerWinScreen()
    {
        if (WinScreen != null)
        {
            Time.timeScale = 0f;
            WinScreen.SetActive(true);
        }
    }

    //Lose Screen
    public void TriggerLoseScreen()
    {
        if (loseScreen != null)
        {
            Time.timeScale = 0f;
            loseScreen.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;

        //Close lose screen
        if (loseScreen != null)
        {
            loseScreen.SetActive(false);
        }
        //Close win screen
        if (WinScreen != null)
        {
            WinScreen.SetActive(false);
        }
    }

    public void ExitGame()
    {
        Application.Quit();

        //Stops play mode in the editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void LoadNextLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        //Check if next scene exists in build settings
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            ResumeGame();
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.Log("No more levels in build settings.");
        }
    }

    public void ReturnToMainMenu()
    {
        ResumeGame();
        SceneManager.LoadScene(0);
    }

    public void RestartLevel()
    {
        ResumeGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}