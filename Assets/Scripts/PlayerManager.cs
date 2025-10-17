using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] PlayerProjectile playerProjectile;
    [SerializeField] Mixers mixers;
    [SerializeField] PlayerInventory playerInventory;
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
        //Check if we have died.
        if (GetHealth() <= 0)
        {
            isAlive = false;
        }
    }
    public void OnTriggerEnter2D(Collider2D other)
    {  
        //Check if we were hit by enemy bullet. Need reference to how much damage it does.
        if (other.CompareTag("EnemyBullet"))
        {
            //TakeDamage(other.getDamage)
            //Need enemy bullet actual damage.^
        }
    }
    //Returns current player health
    public float GetHealth()
    {
        return health;
    }
    //Resets player health to base. To be used on picking up health, or restarting.
    protected void ResetHealth()
    {
        health = baseHealth;
    }
    //Returns current player speed.
    public float GetSpeed()
    {
        return speed;
    }
    ///Reset the speed to the base.
    public void ResetSpeed()
    {
        speed = baseSpeed;
    }

    ///How much more speed should we have? i.e. 1.2 = 20% more speed.
    public void SetSpeedMod(float mod)
    {
        ResetSpeed();
        speed *= mod;
    }
    //Returns how much more damage we are taking as a multiplier.
    public float GetDamageSens()
    {
        return damageSens;
    }
    //Resets the damage sensitivity to base (1).
    public void ResetDamageSens()
    {
        damageSens = baseSensitivity;
    }
    //Take damage from environment, bullet, etc...
    private void TakeDamage(float damage)
    {
        health -= (int)(damageSens * damage);
    }
    //Resets the gun damage multiplier.
    public void ResetDamageMod()
    {
        playerProjectile.SetDamageMod(1);
    }
    //Sets how much more damage should the gun be doing as a multiplier.
    public void SetDamageMod(float mod)
    {
        playerProjectile.SetDamageMod(mod);
    }
    //Resets the cooldown on gun usage.
    public void ResetCooldown()
    {
        playerProjectile.setCooldownMod(1);
    }
    //Sets the cooldown on gun usage.
    public void SetCooldownMod(float mod)
    {
        playerProjectile.setCooldownMod(mod);
    }
    public void SetAccuracyMod(float mod)
    {
        playerProjectile.setAccuracyMod(mod);
    }
    public void ResetAccuracyMod()
    {
        playerProjectile.setAccuracyMod(1);
    }
    public void SetDestroyBulletsOn()
    {
        playerProjectile.setDestroyBullets(true);
    }
}

