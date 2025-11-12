using System;
using UnityEngine;
using UnityEngine.Analytics;
using Unity.Cinemachine;
//The dynamite is a non-moving aoe attack. Although not a "real projectile," it has overlapping features
public class Dynamite : MonoBehaviour
{
    public float max_radius;
    private float curr_radius;
    private float last_time;
    public float time_between;
    public float num_increases;
    public float damage;
    //damage only dealt when isExploding = true;
    private bool isExploding;
    [SerializeField] Transform transform_object;
    [SerializeField] private CinemachineImpulseSource impulseSource;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isExploding = false;
        max_radius = 1f;
        time_between = 0.001f;
        curr_radius = transform_object.localScale.x / 2;
        damage = 1;
        Explode();
    }

    // Update is called once per frame
    void Update()
    {
        if (isExploding)
        {
            if (curr_radius >= max_radius){isExploding = false;}
            else
            {
                float curr_time = Time.time;
                if ((curr_time - last_time) >= time_between)
                {
                    num_increases++;
                    last_time = curr_time;
                    time_between += 0.001f * (float)Math.Sqrt(num_increases);
                    transform_object.localScale += new Vector3(0.1f, 0.1f, 0);
                    curr_radius = transform_object.localScale.x / 2;
                }
            }
        }
    }

    void Explode()
    {
        isExploding = true;
        last_time = Time.time;
        if (impulseSource != null)
            impulseSource.GenerateImpulse(0.5f);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnAttackHit(collision);
    }

     public virtual void OnAttackHit(Collider2D collision)
    {
        if (isExploding && collision.CompareTag("Player"))
            collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
    }

}
