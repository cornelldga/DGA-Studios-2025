using System;
using UnityEngine;
using UnityEngine.Analytics;
using Unity.Cinemachine;
/// <summary>
/// Dynamites that the drill boss spawns
/// </summary>
public class Dynamite : MonoBehaviour
{
    [Tooltip("True when the dynamite is exploding")]
    private bool isExploding;

    public float max_radius;
    private float curr_radius;
    private float last_time;
    public float time_between;
    public float num_increases;
    public float damage;
    [SerializeField] Transform transform_object;
    [SerializeField] private CinemachineImpulseSource impulseSource;

    /// <summary>
    /// Dynamites that the drill boss spawns
    /// </summary>
    void Start()
    {
        isExploding = false;
        max_radius = 1f;
        time_between = 0.001f;
        curr_radius = transform_object.localScale.x / 2;
        damage = 1;
    }


    /// <summary>
    /// On update, if the dynamite is exploding, the affect area increases
    /// </summary>
    void Update()
    {
        if (isExploding)
        {
            if (curr_radius >= max_radius) { isExploding = false; }
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

    /// <summary>
    /// Sets isExplosion() to true and shakes screen.
    /// </summary>
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

    /// <summary>
    /// If contacts player during explosion, then deals damage.
    /// </summary>
    public virtual void OnAttackHit(Collider2D collision)
    {
        if (isExploding && collision.CompareTag("Player"))
            collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
    }

}
