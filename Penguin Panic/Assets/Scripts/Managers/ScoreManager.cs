using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text scoreValueText;
    [SerializeField] private TMP_Text totalScoreValueText1;
    [SerializeField] private TMP_Text totalScoreValueText2;

    private int currentScore = 0;
    private int totalScore = 0;

    //Subscribe to events
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        LevelManager.OnLevelComplete += AddTotalScore;
        OrcaController.OnPlayerCaught += AddTotalScore;
        FishController.OnFishCaught += AddScore;
        SchoolController.OnSchoolCaught += AddScore;
    }

    //Unsubscribe from events
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        LevelManager.OnLevelComplete -= AddTotalScore;
        OrcaController.OnPlayerCaught -= AddTotalScore;
        FishController.OnFishCaught -= AddScore;
        SchoolController.OnSchoolCaught -= AddScore;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Reset scores on main menu
        if (scene.buildIndex == 0)
        {
            currentScore = 0;
            totalScore = 0;
            if (totalScoreValueText1 != null || totalScoreValueText2 != null)
            {
                totalScoreValueText1.gameObject.SetActive(false);
                totalScoreValueText2.gameObject.SetActive(false);
            }
            if (scoreValueText != null || scoreText != null)
            {
                scoreText.gameObject.SetActive(false);
                scoreValueText.gameObject.SetActive(false);
            }
        }
        //Enable score UI on other scenes
        else
        {
            if (totalScoreValueText1 != null && totalScoreValueText2 != null)
            {
                totalScoreValueText1.gameObject.SetActive(true);
                totalScoreValueText2.gameObject.SetActive(true);
            }
            if (scoreValueText != null && scoreText != null)
            {
                scoreText.gameObject.SetActive(true);
                scoreValueText.gameObject.SetActive(true);
            }
        }

        //Reset current score
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
        if (totalScoreValueText1 != null && totalScoreValueText2 != null)
        {
            totalScoreValueText1.text = totalScore.ToString();
            totalScoreValueText2.text = totalScore.ToString();
        }
    }
}
