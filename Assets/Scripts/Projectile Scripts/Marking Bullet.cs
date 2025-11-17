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
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.ApplyMark(markDuration);
            }
        }
        base.OnProjectileHit(collision);
    }
}
