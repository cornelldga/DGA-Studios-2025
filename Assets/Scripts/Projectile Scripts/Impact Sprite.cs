using UnityEngine;
/// <summary>
/// A Sprite that is created at the impact of a projectile and is destroyed after playing the impact animation
/// </summary>
public class ImpactSprite : MonoBehaviour
{
    /// <summary>
    /// Destroys this GameObject
    /// </summary>
    public void DestroyImpact()
    {
        Destroy(gameObject);
    }
}
