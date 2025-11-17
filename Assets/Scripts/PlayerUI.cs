using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player player;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI healthText;

    void Update()
    {
        if (player != null)
        {
            // Display health
            if (healthText != null)
            {
                healthText.text = $"{player.GetHealth():F0}";
            }
            if (player.IsMarked())
            {
                healthText.color = Color.red;
            }
            else
            {
                healthText.color = Color.white;
            }
        }
    }
}
