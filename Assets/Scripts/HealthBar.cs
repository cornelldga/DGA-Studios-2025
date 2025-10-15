using UnityEngine;
using UnityEngine.UIElements;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private BossManager bossManager;
    public Slider slider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = bossManager.getHealth()/bossManager.getMaxHealth();
    }
}
