using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    public int CurrentHealth => currentHealth;

    [Header("Invincibility Settings")]
    [SerializeField] private float invincibilityDuration = 2f;
    private bool isInvincible = true;
    private float invincibilityTimer = 0f;

    public static event Action OnPlayerDeath;

    private void Start()
    {
        currentHealth = maxHealth;
        GameManager.Instance.UIManager.InitHealthUI(maxHealth);
        GameManager.Instance.UIManager.UpdateHealthUI(currentHealth);
    }

    //Update health UI
    private void UpdateHealthUI()
    {
        GameManager.Instance.UIManager.UpdateHealthUI(currentHealth);
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

    public void HealDamage(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        //Update health UI
        UpdateHealthUI();
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
