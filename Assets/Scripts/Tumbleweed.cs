using System;
using UnityEngine;

public class Tumbleweed : Bullet
{

    // stats 

    [SerializeField] private float fireDamage = 5f;
    [SerializeField] private float fireDuration = 1f;

    // sets the current tumbleweed to be on fire (increased damage decreased duration)
    private void setFire()
    {
        damage = fireDamage;
        duration = fireDuration;
        //placeholder until animation set
        GetComponent<SpriteRenderer>().color = Color.red;
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        OnProjectileHit(collision);
    }


    public void FixedUpdate()
    {
        transform.Rotate(Vector3.back * 360f * Time.deltaTime);
    }

    // <summary>
    // collision handling, if interacts with fire should be set on fire
    //</summary>
    public override void OnProjectileHit(Collider2D collision)
    {
        if (collision.CompareTag("Bush"))
        {
            Bush bush = collision.gameObject.GetComponent<Bush>();
            if (bush.isOnFire) setFire();

        }
        else { 
            base.OnProjectileHit(collision);
        }
    }
}
