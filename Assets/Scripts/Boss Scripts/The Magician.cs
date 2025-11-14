using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// The types of stages for The Magician
/// </summary>
public enum Stage
{
    Backstage, Card, Dove, Knife
}
public class TheMagician : Boss
{
    public Stage currentStage;

    [Tooltip("Magician Animation Controller")]
    [SerializeField] private Animator animator;

    [Header("Attack Control Variables")]
    [Tooltip("Number of Attacks per Phase")]
    [SerializeField] float attacksNum;

    [Tooltip("Attack rate during the second phase")]
    [SerializeField] float secondAttackRate;

    [Tooltip("Attack rate during the third phase")]
    [SerializeField] float thirdAttackRate;

    [Header("Movement Control Variables")]
    [Tooltip("How long The Magician hides in the backstage")]
    [SerializeField] float backStageTime;
    private float stageTimer;

    [Tooltip("How long The Magician spends attacking on Stage")]
    [SerializeField] float attackTime;

    [Tooltip("How long the Magician Waits before Teleporting")]
    [SerializeField] float teleportDelay;
    private float teleportDelayTimer;

    [Header("Stages")]
    [SerializeField] Transform backStage;
    [SerializeField] Transform cardStage;
    [SerializeField] Transform doveStage;
    [SerializeField] Transform knifeStage;

    [Header("Bullet Patterns")]
    [SerializeField] BulletPattern cardStageBulletPattern;
    [SerializeField] BulletPattern doveStageBulletPattern;
    [SerializeField] BulletPattern knifeStageBulletPattern;

    [HideInInspector] public bool obscure;

    

    public override void Start()
    {   
        base.Start();
        currentStage = Stage.Backstage;
        attackCooldown = attackTime / attacksNum;
        stageTimer = attackTime;
        teleportDelayTimer = teleportDelay;
    }

    public override void Update()
    {
        attackCooldown -= Time.deltaTime * attackRate;
        stageTimer -= Time.deltaTime;
        if (!isAttacking && attackCooldown <= 0 && stageTimer >= 0)
        {
            Attack();
        }

        if (stageTimer < .5 * backStageTime && currentStage == Stage.Backstage)
        {
            obscure=true;
        }

        if (stageTimer <= 0)
        {
            teleportDelayTimer -= Time.deltaTime * attackRate;
            
            if(teleportDelayTimer <= 0) 
            {
                if (currentStage != Stage.Backstage)
                {
                    stageTimer = backStageTime;
                    teleportDelayTimer = 0;
                    currentStage = Stage.Backstage;
                }
                else
                {
                    stageTimer = attackTime;
                    teleportDelayTimer = teleportDelay;
                    obscure = false;
                    Shuffle();
                    ChooseNewStage();
                }
            }
            
        }
        
            MoveToStage();
        animator.SetBool("isAttacking", isAttacking);
    }

    /// <summary>
    /// Moves The Magician to the current stage
    /// </summary>
    public void MoveToStage()
    {
        switch (currentStage)
        {
            case Stage.Backstage:
                transform.position = backStage.position;
                break;
            case Stage.Card:
                transform.position = cardStage.position;
                break;
            case Stage.Dove:
                transform.position = doveStage.position;
                break;
            case Stage.Knife:
                transform.position = knifeStage.position;
                break;
        }
    }
    /// <summary>
    /// Choose a random stage location from knife, dove, or cards
    /// </summary>
    private void ChooseNewStage()
    {
        List<Stage> stages = new List<Stage>();
        stages.AddRange(Enum.GetValues(typeof(Stage)));
        stages.Remove(Stage.Backstage);
        currentStage = stages[UnityEngine.Random.Range(0, stages.Count)];
    }

    /// <summary>
    /// Shuffles the location of the Stages
    /// </summary>
    public void Shuffle()
    {

    }

    /// <summary>
    /// The Magician executes the attack corresponding to her current stage
    /// </summary>
    /// 
    public override void Attack()
    {
        //ChooseNewStage();
        //MoveToStage();
        base.Attack();
        switch (currentStage)
        {
            case Stage.Backstage:
                //attackCooldown = 0;
                break;
            case Stage.Card:
                StartCoroutine(cardStageBulletPattern.DoBulletPattern(this));
                attackCooldown = attackTime / attacksNum;
                break;
            case Stage.Dove:
                StartCoroutine(doveStageBulletPattern.DoBulletPattern(this));
                attackCooldown = attackTime / attacksNum;
                break;
            case Stage.Knife:
                StartCoroutine(knifeStageBulletPattern.DoBulletPattern(this));
                attackCooldown = attackTime / attacksNum;
                break;
        }
    }
 

    public override void SetPhase(float healthPercent)
    {
        base.SetPhase(healthPercent);
        switch (currentPhase)
        {
            case 1:
                attackRate = secondAttackRate;
                break;
            case 2:
                attackRate = thirdAttackRate;
                break;
        }
    }
}
