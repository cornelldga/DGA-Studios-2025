using System;
using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// Dynamite used by Driller Boss: 
/// Will be thrown into holes (not yet implemented) and creates a ring explosion (circle for now)
/// </summary>
public class Dynamite : Projectile
{
    [SerializeField] double timeBeforeExplosion;
    [SerializeField] float impulseForce;
    [SerializeField] CinemachineImpulseSource impulseSource;
    [SerializeField] GameObject originalExplosion;
    private double timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        timer += Time.deltaTime;
        if (timer> timeBeforeExplosion)
        {
            impulseSource.GenerateImpulse(impulseForce);
            Instantiate(originalExplosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
            
        }
    }

}
