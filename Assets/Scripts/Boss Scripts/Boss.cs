using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;

/// <summary>
/// Base class for all bosses in the game
/// </summary>
public abstract class Boss : MonoBehaviour, IDamageable
{
    [SerializeField] protected float maxHealth;
    protected float health;
    [SerializeField] Image healthBar;
    public Transform bulletOrigin;

    [Tooltip("Specifices the number of additional boss phases (excluding the first phase)" +
        "and the percent health they are triggered")]
    [SerializeField] protected float[] phasePercents;

    protected int currentPhase = 0;

    bool isAttacking;
    protected float attackCooldown;

    [Tooltip("The speed of teh attack cooldown")]
    public float attackRate = 1;

    public virtual void Start()
    {
        health = maxHealth;
    }

    public virtual void Update()
    {
        attackCooldown -= Time.deltaTime * attackRate;
        if (!isAttacking && attackCooldown <= 0)
        {
            Attack();
        }
    }
    /// <summary>
    /// Sets the boss attack state
    /// </summary>
    /// <param name="isAttacking">whether the boss is attacking</param>
    public virtual void SetAttackState(bool isAttacking)
    {
        this.isAttacking = isAttacking;
    }
    /// <summary>
    /// Sets the boss attack cooldown
    /// </summary>
    /// <param name="cooldown">the cooldown in seconds</param>
    /// <returns></returns>
    public void SetAttackCooldown(float cooldown)
    {
        attackCooldown = cooldown;
    }

    /// <summary>
    /// The attack logic for the boss for when it can attack. Points the bullet origin towards
    /// the position of the player
    /// </summary>
    public virtual void Attack()
    {
        bulletOrigin.transform.right = GameManager.Instance.player.transform.position
            - bulletOrigin.transform.position;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            this.enabled = false;
            healthBar.fillAmount = 0;
        }
        else
        {
            float healthPercent = health / maxHealth;
            healthBar.fillAmount = healthPercent;
            CheckPhase(healthPercent);
            
        }
    }
    /// <summary>
    /// Checks if the boss reached a new phase based on health remaining
    /// </summary>
    /// <param name="healthPercent">Percent health remaining</param>
    void CheckPhase(float healthPercent)
    {
        for (int i = currentPhase; i < phasePercents.Length; i++)
        {
            if (healthPercent <= phasePercents[i])
            {
                currentPhase = i;
                SetPhase();
            }
        }
    }

    /// <summary>
    /// Sets the phase of the boss based on its health percentage and updates the logic accordingly
    /// </summary>
    public abstract void SetPhase();
}
