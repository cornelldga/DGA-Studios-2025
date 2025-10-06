using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //How much faster the character should be moving right now as a multiplier.
    private float speedmultiplier;
    //Current speed
    private float speed;
    //Base speed
    private const float base_speed = 7.5f;

    //How much health do we have at this point in time.
    private int health;
    //Base health
    private const int base_health = 100;
    //How much more damage should we recieve as a multiplier.
    private float damage_sens;
    //Basic sensitivity.
    private const int base_sensitivity = 1;

    private bool is_alive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speed = base_speed;
        health = base_health;
        damage_sens = base_sensitivity;
        is_alive = true;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyBullet"))
        {
            health -= 5;
        }
    }

    public float GetSpeed()
    {
        return speed;
    }
    public float GetHealth()
    {
        return health;
    }
    public float GetDamageSens()
    {
        return damage_sens;
    }
    private void ResetSpeed()
    {
        speed = base_speed;
    }
     private void ResetDamage()
    {
        speed = base_sensitivity;
    }
    public void setSpeedMod(float mod)
    {
        ResetSpeed();
        speed *= mod;
    }
    public void setDamageMod(float mod)
    {
        ResetDamage();
        damage_sens *= mod;
    }
}
