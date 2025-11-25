using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;
    [Header("Invincibility Settings")]
    [SerializeField] private float invincibilityDuration = 2f;
    private bool isInvincible = true;
    private float invincibilityTimer = 0f;

    [Header("UI")]
    [SerializeField] private GameObject healthStarPrefab;
    [SerializeField] private GameObject healthStarContainer;
    private List<GameObject> healthStars = new();
    [SerializeField] private Sprite healthStar;
    [SerializeField] private Sprite healthStarEmpty;

    public static event Action OnPlayerDeath;

    private void Start()
    {
        // Initialize health UI
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject star = Instantiate(healthStarPrefab, healthStarContainer.transform);
            healthStars.Add(star);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool hasHUD = LevelManager.Instance != null && LevelManager.Instance.HasHUD;

        healthStarContainer.gameObject.SetActive(hasHUD);

        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    //Update health UI
    private void UpdateHealthUI()
    {
        for (int i = 0; i < healthStars.Count; i++)
        {
            if (i < currentHealth)
            {
                healthStars[i].GetComponent<UnityEngine.UI.Image>().sprite = healthStar;
            }
            else
            {
                healthStars[i].GetComponent<UnityEngine.UI.Image>().sprite = healthStarEmpty;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;
        currentHealth -= damage;

        //Update health UI
        UpdateHealthUI();

        //Die
        if (currentHealth <= 0)
        {
            Die();
        }

        //Become invincible
        else
        {
            isInvincible = true;
            invincibilityTimer = invincibilityDuration;
        }
    }

    private void Die()
    {
        OnPlayerDeath?.Invoke();
    }

    private void FixedUpdate()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.fixedDeltaTime;
            if (invincibilityTimer <= 0f)
            {
                isInvincible = false;
            }
        }
    }
}
