using UnityEngine;

public class DeveloperCheats : MonoBehaviour
{
    [Header("Cheat Settings")]
    [SerializeField] private KeyCode nextLevelKey = KeyCode.N;
    [SerializeField] private KeyCode healPlayerKey = KeyCode.M;
    [SerializeField] private int healAmount = 1;

    void Update()
    {
        // Go to next level
        if (Input.GetKeyDown(nextLevelKey))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadNextLevel();
                Debug.Log("Developer cheat: Load next level");
            }
        }

        // Heal player
        if (Input.GetKeyDown(healPlayerKey))
        {
            if (GameManager.Instance != null && GameManager.Instance.Player != null)
            {
                PlayerHealth health = GameManager.Instance.Player.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    health.HealDamage(healAmount);
                    Debug.Log("Developer cheat: Heal player by " + healAmount);
                }
            }
        }
    }
}
