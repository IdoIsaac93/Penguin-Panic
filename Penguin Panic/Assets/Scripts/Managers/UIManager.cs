using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("Container")]
    [SerializeField] private GameObject uiContainer;

    [Header("Screens")]
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;
    [SerializeField] private GameObject pauseScreen;

    [Header("Score UI")]
    [SerializeField] private TMP_Text scoreValueText;
    [SerializeField] private List<TMP_Text> totalScoreValueText;

    [Header("Health UI")]
    [SerializeField] private GameObject healthStarPrefab;
    [SerializeField] private GameObject healthStarContainer;
    [SerializeField] private Sprite healthStar;
    [SerializeField] private Sprite healthStarEmpty;

    [Header("Timer UI")]
    [SerializeField] private TMP_Text timerText;

    private List<GameObject> healthStars = new();

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool hasHUD = LevelManager.Instance != null && LevelManager.Instance.HasHUD;

        //Toggle HUD elements
        uiContainer.SetActive(hasHUD);
    }

    public void OpenWinScreen()
    {
        if (winScreen != null)
        {
            winScreen.SetActive(true);
        }
    }

    public void OpenLoseScreen()
    {
        if (loseScreen != null)
        {
            loseScreen.SetActive(true);
        }
    }

    public bool TogglePauseMenu()
    {
        if (pauseScreen != null)
        {
            if (pauseScreen.activeSelf)
            {
                pauseScreen.SetActive(false);
                return false;
            }
            else
            {
                pauseScreen.SetActive(true);
                return true;
            }
        }
        return false;
    }

    public void CloseAllScreens()
    {
        if (winScreen != null) winScreen.SetActive(false);
        if (loseScreen != null) loseScreen.SetActive(false);
        if (pauseScreen != null) pauseScreen.SetActive(false);
    }

    //Score UI
    public void UpdateScore(int currentScore)
    {
        if (scoreValueText != null)
            scoreValueText.text = currentScore.ToString();
    }

    public void UpdateTotalScore(int totalScore)
    {
        foreach (var textElement in totalScoreValueText)
        {
            if (textElement != null)
                textElement.text = totalScore.ToString();
        }
    }

    //Health UI
    public void InitHealthUI(int maxHealth)
    {
        healthStars.Clear();
        foreach (Transform child in healthStarContainer.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < maxHealth; i++)
        {
            GameObject star = Instantiate(healthStarPrefab, healthStarContainer.transform);
            healthStars.Add(star);
        }
    }

    public void UpdateHealthUI(int currentHealth)
    {
        for (int i = 0; i < healthStars.Count; i++)
        {
            var image = healthStars[i].GetComponent<UnityEngine.UI.Image>();
            image.sprite = (i < currentHealth) ? healthStar : healthStarEmpty;
        }
    }

    //Timer UI
    public void UpdateTimerUI(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time - minutes * 60);
        timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }
}
