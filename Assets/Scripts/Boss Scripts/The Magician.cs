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
    private int attackCount;

    [Tooltip("Attack rate during the second phase")]
    [SerializeField] float secondAttackRate;

    [Tooltip("Attack rate during the third phase")]
    [SerializeField] float thirdAttackRate;

    [Header("Movement Control Variables")]
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
    [SerializeField] BulletPattern DesperationAttack;
    [Tooltip("How long The Magician hides in the backstage")]
    [SerializeField] float backStageTime;

    

    public override void Start()
    {   
        base.Start();
        currentStage = Stage.Backstage;
        attackCount = 0;
        stageTimer = attackTime;
        teleportDelayTimer = teleportDelay;
    }

    public override void Update()
    {
        
        stageTimer -= Time.deltaTime;
        if ( attackCount < attacksNum*attackRate)
        {
            base.Update();
        }
        else
        {
            attackCooldown -= Time.deltaTime * attackRate;
        }

        if (stageTimer <= 0 && attacksNum * attackRate == attackCount)
        {
            teleportDelayTimer -= Time.deltaTime * attackRate;

            if (teleportDelayTimer <= 0)
            {
                if (currentStage != Stage.Backstage)
                {
                    stageTimer = backStageTime;
                    teleportDelayTimer = 0;
                    currentStage = Stage.Backstage;
                    attackCount = 0;
                }
                else
                {
                    stageTimer = attackTime;
                    teleportDelayTimer = teleportDelay;
                    ChooseNewStage();
                    attackCount = 0;
                }
            }

        }
        
            MoveToStage();
    }

    public override void SetAttackState(bool isAttacking)
    {
        base.SetAttackState(isAttacking);
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
    /// The Magician executes the attack corresponding to her current stage
    /// </summary>
    /// 
    public override void Attack()
    {
        //ChooseNewStage();
        //MoveToStage();
        base.Attack();
        attackCount++;
        switch (currentStage)
        {
            case Stage.Backstage:
                //attackCooldown = 0;
                break;
            case Stage.Card:
                StartCoroutine(cardStageBulletPattern.DoBulletPattern(this));

                break;
            case Stage.Dove:
                StartCoroutine(doveStageBulletPattern.DoBulletPattern(this));

                break;
            case Stage.Knife:
                StartCoroutine(knifeStageBulletPattern.DoBulletPattern(this));

                break;
        }
    }
 

    public override void SetPhase()
    {
        switch (currentPhase)
        {
            case 0:
                attackRate = secondAttackRate;
                break;
            case 1:
                attackRate = thirdAttackRate;
                switch (currentStage)
                {
                    case Stage.Backstage:
                        //attackCooldown = 0;
                        break;
                    case Stage.Card:
                        StopCoroutine(cardStageBulletPattern.DoBulletPattern(this));

                        break;
                    case Stage.Dove:
                        StopCoroutine(doveStageBulletPattern.DoBulletPattern(this));

                        break;
                    case Stage.Knife:
                        StopCoroutine(knifeStageBulletPattern.DoBulletPattern(this));

                        break;
                }
                StartCoroutine(DesperationAttack.DoBulletPattern(this));
                break;
        }
    }
}
