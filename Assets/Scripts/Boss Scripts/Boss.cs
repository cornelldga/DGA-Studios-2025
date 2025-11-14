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

    public bool isAttacking;
    [HideInInspector] public float attackCooldown;

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
            SetPhase(healthPercent);
        }
    }
    /// <summary>
    /// Sets the phase of the bossbased on its health percentage and updates the logic accordingly
    /// </summary>
    /// <param name="healthPercent"></param>
    public virtual void SetPhase(float healthPercent)
    {
        for(int i = 0; i < phasePercents.Length; i++)
        {
            if(currentPhase < i)
            {
                if(healthPercent <= phasePercents[i])
                {
                    currentPhase = i;
                }
            }
        }
    }
}
