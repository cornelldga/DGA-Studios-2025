using System.Collections;
using UnityEngine;
/// <summary>
/// Defines a boss's bullet pattern when they attack. The indexes of the
/// arrays will be cycled through until the largest array is iterated through
/// </summary>
[CreateAssetMenu(fileName = "New Bullet Pattern", menuName = "Bullet Pattern")]
public class BulletPattern : ScriptableObject
{
    [SerializeField] Bullet[] bullets;
    [SerializeField] float[] bulletAngles;
    [SerializeField] float[] bulletDelays;
    [Range(0, 10)]
    [SerializeField] float minAttackCooldown;
    [Range(0, 10)]
    [SerializeField] float maxAttackCooldown;

    /// <summary>
    /// Executes the bullet pattern, preventing the boss from attacking while it cycles through
    /// its attack. When the attack is complete, a random cooldown will be applied to the boss
    /// </summary>
    /// <param name="boss"></param>
    /// <returns></returns>
    public IEnumerator DoBulletPattern(Boss boss)
    {
        boss.isAttacking = true;
        int arrayLength = Mathf.Max(bullets.Length, bulletAngles.Length, bulletDelays.Length);
        for(int i = 0; i < arrayLength; i++)
        {
            yield return new WaitForSeconds(bulletDelays[i%bulletDelays.Length]);
            Bullet bullet = Instantiate(bullets[i % bullets.Length], boss.bulletOrigin.position, boss.bulletOrigin.rotation);
            bullet.transform.Rotate(0,0, bulletAngles[i % bulletAngles.Length], Space.Self);

        }
        boss.attackCooldown = Random.Range(minAttackCooldown, maxAttackCooldown);
        boss.isAttacking = false;
    }
}
