using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("Score UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text scoreValueText;
    [SerializeField] private List<TMP_Text> totalScoreValueText;

    [Header("Health UI")]
    [SerializeField] private GameObject healthStarPrefab;
    [SerializeField] private GameObject healthStarContainer;
    [SerializeField] private Sprite healthStar;
    [SerializeField] private Sprite healthStarEmpty;

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
        scoreText.gameObject.SetActive(hasHUD);
        scoreValueText.gameObject.SetActive(hasHUD);
        healthStarContainer.gameObject.SetActive(hasHUD);
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
}
