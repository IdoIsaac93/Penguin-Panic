using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Singelton
    public static GameManager Instance { get; private set; }

    //Globally accesible references
    [SerializeField] private GameObject player;
    [SerializeField] private UIManager uiManager;
    public GameObject Player => player;
    public UIManager UIManager => uiManager;

    //Level Timer
    private float levelTimer = 0;
    public float LevelTimer => levelTimer;

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
        levelTimer = 0f;
    }

    private void Update()
    {
        //Update level timer
        if (LevelManager.Instance.HasHUD)
        {
            levelTimer += Time.deltaTime;
            if (uiManager != null)
            {
                uiManager.UpdateTimerUI(levelTimer);
            }
        }
    }

    //Win Screen
    public void TriggerWinScreen()
    {
        if (uiManager != null)
        {
            Time.timeScale = 0f;
            uiManager.OpenWinScreen();
        }
    }

    //Lose Screen
    public void TriggerLoseScreen()
    {
        if (uiManager != null)
        {
            Time.timeScale = 0f;
            uiManager.OpenLoseScreen();
        }
    }

    //Pause Screen
    public void PauseGame()
    {
        if (uiManager != null)
        {
            //Pause
            if (uiManager.TogglePauseMenu())
            {
                Time.timeScale = 0f;
            }
            //Resume
            else
            {
                ResumeGame();
            }
        }
    }

    public void ResumeGame()
    {
        if (uiManager != null)
        {
            Time.timeScale = 1f;
            uiManager.CloseAllScreens();
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