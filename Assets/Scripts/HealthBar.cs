using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UIElements;

public class HealthBar : MonoBehaviour
{
    private BossManager bossManager;
    public Slider slider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bossManager = GameObject.Find("Boss").GetComponent<BossManager>();
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bossManager.getMaxHealth() != 0)
        {
            slider.SetValueWithoutNotify


                ( bossManager.getHealth() / bossManager.getMaxHealth());
        }
    }
}
