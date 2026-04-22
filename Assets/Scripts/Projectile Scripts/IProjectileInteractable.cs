/// <summary>
/// Interface that allows interaction with a projectile
/// </summary>
public interface IProjectileInteractable
{
    /// <summary>
    /// Triggers an interaction with the affected projectile
    /// </summary>
    /// <param name="projectile">The projectile to be affected</param>
    /// <returns>If the projectile should call OnProjectileHit</returns>
    public bool ProjectileInteraction(Projectile projectile);
}
