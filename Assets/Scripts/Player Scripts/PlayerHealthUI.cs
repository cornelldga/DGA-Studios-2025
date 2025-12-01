using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("References")]
    private Player player;
    [SerializeField] private Image healthImage;

    [Header("Health Images")]
    [SerializeField] private Sprite healthySprite; // Image for health > 80%
    [SerializeField] private Sprite lowHealthSprite; // Image for health <= 80%

    [Header("Alternating Settings")]
    [SerializeField] private float alternateSpeed = 0.5f; // Time between image switches

    private float lastCheckedHealth;
    private float alternateThreshold; // Will be set to 80% of max health
    private float criticalThreshold; // Will be set to 40% of max health
    private bool isAlternating = false;
    private Coroutine alternateCoroutine;

    void Start()
    {
        player = GameManager.Instance.player;
        alternateThreshold = player.GetHealth() * 0.8f; // Set threshold to 80% of max health
        criticalThreshold = player.GetHealth() * 0.4f; // Set threshold to 40% of max health
        UpdateHealthImage(player.GetHealth());
    }

    void Update()
    {
        if (player != null)
        {
            float currentHealth = player.GetHealth();

            // Only update if health actually changed
            if (currentHealth != lastCheckedHealth) //prevent many calls to the health image update.
            {
                UpdateHealthImage(currentHealth);
                lastCheckedHealth = currentHealth; 
            }
        }
    }
    /// <summary>
    /// State machine for the health UI. If health falls below a set threshold, the alternating image will begin. 
    /// If the health falls below a critical threshold, then image sticks to the wounded image.
    /// </summary>
    private void UpdateHealthImage(float currentHealth)
    {
        if (healthImage == null)
            return;

        if (currentHealth <= criticalThreshold)
        {
            // Below 40% - stop alternating and show low health sprite
            if (isAlternating)
            {
                isAlternating = false;
                if (alternateCoroutine != null)
                {
                    StopCoroutine(alternateCoroutine);
                }
            }
            healthImage.sprite = lowHealthSprite;
        }
        else if (currentHealth <= alternateThreshold)
        {
            // Between 40% and 80% - alternate between sprites
            if (!isAlternating)
            {
                isAlternating = true;
                alternateCoroutine = StartCoroutine(AlternateHealthImages());
            }
        }
        else
        {
            // Above 80% - stop alternating and show healthy sprite
            if (isAlternating)
            {
                isAlternating = false;
                if (alternateCoroutine != null)
                {
                    StopCoroutine(alternateCoroutine);
                }
            }
            healthImage.sprite = healthySprite;
        }
    }
    /// <summary>
    /// Handles logic for swapping the image for the health UI.
    /// </summary>
    private IEnumerator AlternateHealthImages()
    {
        while (isAlternating)
        {
            healthImage.sprite = lowHealthSprite;
            yield return new WaitForSeconds(alternateSpeed);
            healthImage.sprite = healthySprite;
            yield return new WaitForSeconds(alternateSpeed);
        }
    }
}
