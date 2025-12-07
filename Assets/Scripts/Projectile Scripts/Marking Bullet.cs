using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// Projectiles that the bosses shoot
/// </summary>

public class MarkingBullet : Bullet
{
    [Tooltip("Duration the player will be marked")]
    [SerializeField] private float markDuration = 5f;

    public override void OnProjectileHit(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameObject[] pigs = GameObject.FindGameObjectsWithTag("Pig");
            foreach(GameObject pig in pigs)
            {
                pig.GetComponent<Pig>().ChargeTarget(GameManager.Instance.player.transform.position);
            }
        } else if (Whipped() && collision.CompareTag("Enemy"))
        {
            PigRider pigRider = collision.gameObject.GetComponent<PigRider>();
            if (pigRider != null)
            {
                pigRider.ApplyMark(markDuration);
            }
        }
        base.OnProjectileHit(collision);
    }
}
