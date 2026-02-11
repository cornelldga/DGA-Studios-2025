using System;
using UnityEngine;

public class Tumbleweed : Bullet
{

    // stats 

    [SerializeField] private float fireDamage = 5f;
    [SerializeField] private float fireDuration = 3f;

    


    // sets the current tumbleweed to be on fire (increased damage decreased duration)
    private void setFire()
    {
        damage = fireDamage;
        duration = fireDuration;
        GetComponent<Animator>().SetBool("onFire", true); 
    }


public void OnTriggerEnter2D(Collider2D collision)
{
    OnProjectileHit(collision);
}

// <summary>
// collision handling, if interacts with fire should be set on fire, should damage player
//</summary>
public override void OnProjectileHit(Collider2D collision)
{
    if (collision.gameObject.CompareTag("onFire"))
    {
        setFire();
    }
}
}
