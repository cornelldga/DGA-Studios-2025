using UnityEngine;

/// <summary>
/// A Base is a projectile that a player equips and fires, using mixer modifiers to affect certain projectile properties
/// </summary>
public class Base : Projectile
{
    public float cooldown;
    [SerializeField] Sprite baseSprite;

    public Sprite getSprite()
    {
        return baseSprite;
    }
}