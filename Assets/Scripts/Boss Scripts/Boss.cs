using TMPro;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;

/// <summary>
/// Base class for all bosses in the game
/// </summary>
public abstract class Boss : MonoBehaviour, IDamageable
{
    [Tooltip("What the boss health bar name is set to")]
    [SerializeField] string bossName;
    [SerializeField] protected float maxHealth;
    protected float health;
    [SerializeField] Image healthBar;
    [SerializeField] TMP_Text bossNameText;
    public Transform bulletOrigin;

    [Tooltip("Specifices the number of additional boss phases (excluding the first phase)" +
        "and the percent health they are triggered")]
    [SerializeField] protected float[] phasePercents;

    [SerializeField] protected int currentPhase = 0;

    bool isAttacking;
    protected float attackCooldown;

    [Tooltip("The speed of the attack cooldown")]
    [SerializeField] protected float attackRate = 1;

    public virtual void Start()
    {
        bossNameText.text = bossName;
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

    public virtual void TakeDamage(float damage)
    {
        Debug.Log(health);
        Debug.Log("HEALLTH");
        health -= damage;
        if(health <= 0)
        {
            
            healthBar.fillAmount = 0;
            GameManager.Instance.BossDefeated("World Hub");
        }
        else
        {
            float healthPercent = health / maxHealth;
            // healthBar.fillAmount = healthPercent;
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
                currentPhase = i+1;
                SetPhase();
            }
        }
    }

    /// <summary>
    /// Sets the phase of the boss based on its health percentage and updates the logic accordingly
    /// </summary>
    public abstract void SetPhase();
}
