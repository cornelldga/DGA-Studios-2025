using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HealthBar : MonoBehaviour
{
    private BossManager bossManager;
    public UnityEngine.UI.Slider healthSlider;
    [SerializeField] string bossName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bossManager = GameObject.Find(bossName).GetComponent<BossManager>();
        healthSlider.maxValue = bossManager.getMaxHealth();
    }

    // Update is called once per frame
    void Update()
    {
        if (bossManager.getMaxHealth() != 0)
        {

            healthSlider.SetValueWithoutNotify(bossManager.getHealth());

        }
    }
}
