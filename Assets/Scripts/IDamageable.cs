using UnityEngine;
/// <summary>
/// The damage interface for all objects that take damage in the game
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// Takes the specified damage and checks damage conditions
    /// </summary>
    /// <param name="damage"></param>
     public void TakeDamage(float damage);
}
