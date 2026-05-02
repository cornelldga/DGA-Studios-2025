using System;
using UnityEngine;

public class Contract : MonoBehaviour, IDamageable
{
    public GameObject boss;
    public Granny granny;

    public float maxHealth = 2f;

    private float health;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        Boss bossScript = boss.GetComponent<Boss>();
        health = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            // Destroys contract and disables boss
            Destroy(gameObject);
            granny.bossActive = false;
            Destroy(boss);

            // Removes boss from Granny contracts
            granny.GetAvailableBoss().Remove(boss);
            granny.contractDestroyed = true;
            // Locks Granny from double contract animation when contract is destroyed
            if (granny.GetAvailableBoss().Count > 0)
            {
                granny.LockDouble();
            }

            // Transitions and granny takes damage
            granny.TransitionToReturning();
            granny.TakeDamageFromContract();
        }
    }
}
