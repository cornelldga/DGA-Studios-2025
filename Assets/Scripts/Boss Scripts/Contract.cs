using System;
using UnityEngine;

public class Contract : MonoBehaviour, IDamageable
{
    public GameObject boss;
    public Granny granny;

    public float maxHealth = 0;

    private float health;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        Boss bossScript = boss.GetComponent<Boss>();
        // Set text to 
        bossScript.getName();
        
        health = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            // TODO Destroy contract logic here
        }
    }   
}
