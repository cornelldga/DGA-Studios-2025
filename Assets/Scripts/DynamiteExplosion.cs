using System;
using UnityEngine;
using Unity.Cinemachine;

public class DynamiteExplosion : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float explosionDuration;
    private double timer;
    
    void Start()
    {
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > explosionDuration)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
        // if (collision.CompareTag("Hole")); //do something
        
    }

}
