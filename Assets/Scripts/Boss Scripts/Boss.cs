using System;
using System.Collections.Generic;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;

/// <summary>
/// Base class for all bosses in the game
/// </summary>
public abstract class Boss : MonoBehaviour, IDamageable
{
    protected Animator animator;
    [Tooltip("The progression value for defeating this boss")]
    [SerializeField] int bossProgression;
    [Tooltip("What the boss health bar name is set to")]
    [SerializeField] string bossName;
    [SerializeField] protected float maxHealth;
    protected float health;
    [SerializeField] Image healthBar;
    [SerializeField] Animator healthBarAnimator;
    protected Rigidbody2D rb;
    [SerializeField] TMP_Text bossNameText;
    public Transform bulletOrigin;

    [Tooltip("Specifices the number of additional boss phases (excluding the first phase)" +
        "and the percent health they are triggered")]
    [SerializeField] protected float[] phasePercents;

    [SerializeField] protected int currentPhase = 0;

    public bool isInvulnerable = false;
    public bool isSummoned = false;

    bool isAttacking;
    protected float attackCooldown;

    [Tooltip("The speed of the attack cooldown")]
    [SerializeField] protected float attackRate = 1;

    [Header("Defeat Dialogue")]
    [SerializeField] private TextAsset defeatDialogue;
    [SerializeField] private Sprite dialogueBoxSprite;
    private Dictionary<DialogueEmotion, Sprite> emotionDictionary = new Dictionary<DialogueEmotion, Sprite>();
    [SerializeField] private Sprite neutralSprite;
    [SerializeField] private Sprite happySprite;
    [SerializeField] private Sprite sadSprite;
    [SerializeField] bool customTextColor;
    [SerializeField] Color textColor;

    bool defeated;

    public virtual void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        bossNameText.text = bossName;
        health = maxHealth;
        SetHealthBar();
        emotionDictionary[DialogueEmotion.Neutral] = neutralSprite;
        emotionDictionary[DialogueEmotion.Happy] = happySprite;
        emotionDictionary[DialogueEmotion.Sad] = sadSprite;
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
        Vector3 dir = GameManager.Instance.player.transform.position
            - bulletOrigin.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        bulletOrigin.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    /// <summary>
    /// Sets the health bar and returns the health percentage
    /// </summary>
    protected float SetHealthBar()
    {
        float healthPercent = health / maxHealth;
        healthBar.fillAmount = healthPercent;
        return healthPercent;
    }

    public virtual void TakeDamage(float damage)
    {
        if (isInvulnerable) return;
        health -= damage;
        if (health <= 0)
        {
            healthBar.fillAmount = 0;
            Defeat();
            return;
        }
        else
        {
            SetHealthBar();
            CheckPhase(SetHealthBar());

        }
    }
    /// <summary>
    /// Called when the boss is defeated. Set the progression of the player, make the player invulnerable,
    /// and play the death animation
    /// </summary>
    public virtual void Defeat()
    {
        GameManager.Instance.player.SetInvulnerable(true);
        PlayerPrefs.SetInt("progression", Mathf.Max(
            PlayerPrefs.GetInt("progression", 0), bossProgression
        ));
        animator.SetTrigger("Defeat");
        rb.simulated = false;
        this.enabled = false;


    }
    /// <summary>
    /// Called when the boss death animation is complete. Triggers dialogue and brings player back to World Hub.
    /// Sets the character's name to the boss name before the ','
    /// </summary>
    public virtual void AnimationBossDeathComplete()
    {
        GameManager.Instance.GetDialogueManager.StartDialogue(defeatDialogue, dialogueBoxSprite, emotionDictionary,
            bossName.Substring(0, bossName.IndexOf(',')), DialogueType.SceneChange, "World Hub", customTextColor ? textColor : null);
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
                currentPhase = i + 1;
                SetPhase();
            }
        }
    }

    /// <summary>
    /// Sets the phase of the boss based on its health percentage and updates the logic accordingly
    /// </summary>
    public virtual void SetPhase()
    {
        healthBarAnimator.SetTrigger("PhaseChange");
    }
}
