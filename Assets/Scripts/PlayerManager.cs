using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //Current speed
    private float speed;
    //Base speed
    private const float baseSpeed = 7.5f;

    //How much health do we have at this point in time.
    private int health;
    //Base health
    private const int baseHealth = 100;
    //How much more damage should we recieve as a multiplier.
    private float damageSens;
    //Basic sensitivity.
    private const int baseSensitivity = 1;

    private bool isAlive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speed = baseSpeed;
        health = baseHealth;
        damageSens = baseSensitivity;
        isAlive = true;
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
        return damageSens;
    }
    ///Reset the speed to the base.
    ///
    private void ResetSpeed()
    {
        speed = baseSpeed;
    }
     private void ResetDamage()
    {
        speed = baseSensitivity;
    }
    ///How much more speed should we have? i.e. 1.2 = 20% more speed.
    ///
    public void SetSpeedMod(float mod)
    {
        ResetSpeed();
        speed *= mod;
    }
    ///How much more damage should we take? i.e. 1.2 = 20% more damage.
    ///
    public void SetDamageMod(float mod)
    {
        ResetDamage();
        damageSens *= mod;
    }
}
