using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("References")]
    private Player player;
    [SerializeField] private Image healthImage;

    [Header("Health Images")]
    [SerializeField] private Sprite healthySprite; // Image for health > 40%
    [SerializeField] private Sprite lowHealthSprite; // Image for health <= 40%

    private float lastCheckedHealth;
    private float lowHealthThreshold; // Will be set to 40% of max health

    void Start()
    {
        player = GameManager.Instance.player;
        lowHealthThreshold = player.GetHealth() * 0.4f; // Set threshold to 40% of max health
        UpdateHealthImage(player.GetHealth());
    }

    void Update()
    {
        if (player != null)
        {
            float currentHealth = player.GetHealth();

            // Only update if health actually changed
            if (currentHealth != lastCheckedHealth)
            {
                UpdateHealthImage(currentHealth);
                lastCheckedHealth = currentHealth;
            }
        }
    }

    private void UpdateHealthImage(float currentHealth)
    {
        if (healthImage == null)
            return;

        if (currentHealth <= lowHealthThreshold)
        {
            healthImage.sprite = lowHealthSprite;
        }
        else
        {
            healthImage.sprite = healthySprite;
        }
    }
}
