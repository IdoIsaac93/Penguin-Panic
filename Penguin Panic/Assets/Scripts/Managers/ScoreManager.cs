using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    private int currentScore = 0;
    private int totalScore = 0;
    private int failedLevelScore = 0;

    //Subscribe to events
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        LevelManager.OnLevelComplete += AddTotalScore;
        PlayerHealth.OnPlayerDeath += OnLose;
        FishController.OnFishCaught += AddScore;
        SchoolController.OnSchoolCaught += AddScore;
    }

    //Unsubscribe from events
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        LevelManager.OnLevelComplete -= AddTotalScore;
        PlayerHealth.OnPlayerDeath -= OnLose;
        FishController.OnFishCaught -= AddScore;
        SchoolController.OnSchoolCaught -= AddScore;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool hasHUD = LevelManager.Instance != null && LevelManager.Instance.HasHUD;

        if (!hasHUD)
        {
            totalScore = 0;
        }

        currentScore = 0;
        GameManager.Instance.UIManager.UpdateScore(currentScore);
    }

    private void Start()
    {
        GameManager.Instance.UIManager.UpdateScore(currentScore);
    }

    private void AddScore(int amount)
    {
        currentScore += amount;
        GameManager.Instance.UIManager.UpdateScore(currentScore);
    }

    private void AddTotalScore()
    {
        totalScore += currentScore;
        GameManager.Instance.UIManager.UpdateTotalScore(totalScore);
    }

    private void OnLose()
    {
        AddTotalScore();
        failedLevelScore = currentScore;
    }

    public void HandleRetryLevel()
    {
        totalScore -= failedLevelScore;
        failedLevelScore = 0;
        GameManager.Instance.UIManager.UpdateTotalScore(totalScore);
    }
}
