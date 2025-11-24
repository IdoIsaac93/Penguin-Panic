using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text scoreValueText;
    [SerializeField] private List<TMP_Text> totalScoreValueText;

    private int currentScore = 0;
    private int totalScore = 0;
    private int failedLevelScore = 0;

    //Subscribe to events
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        LevelManager.OnLevelComplete += AddTotalScore;
        OrcaController.OnPlayerCaught += HandlePlayerCaught;
        FishController.OnFishCaught += AddScore;
        SchoolController.OnSchoolCaught += AddScore;
    }

    //Unsubscribe from events
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        LevelManager.OnLevelComplete -= AddTotalScore;
        OrcaController.OnPlayerCaught -= HandlePlayerCaught;
        FishController.OnFishCaught -= AddScore;
        SchoolController.OnSchoolCaught -= AddScore;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool hasScore = LevelManager.Instance != null && LevelManager.Instance.HasScore;

        scoreText.gameObject.SetActive(hasScore);
        scoreValueText.gameObject.SetActive(hasScore);

        if (!hasScore)
        {
            totalScore = 0;
        }

        currentScore = 0;
        UpdateScoreUI();
    }

    private void Start()
    {
        UpdateScoreUI();
    }

    private void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScoreUI();
    }

    private void AddTotalScore()
    {
        totalScore += currentScore;
        UpdateTotalScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreValueText != null)
        {
            scoreValueText.text = currentScore.ToString();
        }
    }

    private void UpdateTotalScoreUI()
    {
        if (totalScoreValueText != null && totalScoreValueText.Count > 0)
        {
            foreach (var textElement in totalScoreValueText)
            {
                if (textElement != null)
                {
                    textElement.text = totalScore.ToString();
                }
            }
        }
    }

    private void HandlePlayerCaught()
    {
        AddTotalScore();
        failedLevelScore = currentScore;
    }

    public void HandleRetryLevel()
    {
        totalScore -= failedLevelScore;
        failedLevelScore = 0;
        UpdateTotalScoreUI();
    }
}
