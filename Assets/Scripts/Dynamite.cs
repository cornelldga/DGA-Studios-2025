using System;
using Unity.Cinemachine;
using UnityEngine;

public class Dynamite : Projectile
{
    [SerializeField] double timeBeforeEplosion;
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
        if (timer> timeBeforeEplosion)
        {
            impulseSource.GenerateImpulse(impulseForce);
            Instantiate(originalExplosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
            
        }
    }

}
