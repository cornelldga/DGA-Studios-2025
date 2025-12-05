using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

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

    private Vector3 ogCard;
    private Vector3 ogDove;
    private Vector3 ogKnife;

    [Header("Bullet Patterns")]
    [SerializeField] BulletPattern cardStageBulletPattern;
    [SerializeField] BulletPattern doveStageBulletPattern1;
    [SerializeField] BulletPattern doveStageBulletPattern2;
    [SerializeField] BulletPattern knifeStageBulletPattern;
    [SerializeField] BulletPattern DesperationAttack;
    [Tooltip("How long The Magician hides in the backstage")]
    [SerializeField] float backStageTime;
    [Tooltip("How long The Magician waits before attacking")]
    [SerializeField] float appearDelay = 2f;
    private float appearDelayTimer;




    public override void Start()
    {   
        base.Start();
        currentStage = Stage.Backstage;
        stageTimer = attackTime;
        teleportDelayTimer = teleportDelay;

        ogCard = cardStage.position;
        ogKnife = knifeStage.position;
        ogDove = doveStage.position;
        appearDelayTimer = 0;

        animator.SetFloat("aR", attackRate);
        animator.SetFloat("teleportDelayTimer", teleportDelayTimer);
    }
    
    public override void Update()
    {
        animator.SetFloat("aR", attackRate);
        animator.SetFloat("teleportDelayTimer", teleportDelayTimer);

        if (appearDelay / (currentPhase + 1) < appearDelayTimer)
        {
            base.Update();
            stageTimer -= Time.deltaTime;
          
            if (stageTimer <= 0)
            {
                teleportDelayTimer -= Time.deltaTime * attackRate;
                if (teleportDelayTimer <= 0)
                {
                    if (currentStage != Stage.Backstage)
                    {
                        stageTimer = backStageTime;
                        teleportDelayTimer = 0;
                        currentStage = Stage.Backstage;
                        Shuffle();
                    }
                    else
                    {
                        stageTimer = attackTime;
                        teleportDelayTimer = teleportDelay;
                        appearDelayTimer = 0;
                        ChooseNewStage();
                    }
                }
            }
            MoveToStage();
        }
        else
        {
            appearDelayTimer += Time.deltaTime;
        }
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
    /// Shuffles stage locations
    /// </summary>
    private void Shuffle()
    {
        List<Stage> stages = new List<Stage>();
        stages.AddRange(Enum.GetValues(typeof(Stage)));
        stages.Remove(Stage.Backstage);
        Stage cStage;
        Vector3[] pos = { ogCard, ogDove, ogKnife };

        for (int i = 0; i < 3; i++)
        {
            cStage = stages[UnityEngine.Random.Range(0, stages.Count)];
            stages.Remove(cStage);
            switch (cStage)
            {
                case Stage.Card:
                    cardStage.position= pos[i];
                    break;
                case Stage.Dove:
                    doveStage.position = pos[i];
                    break;
                case Stage.Knife:
                    knifeStage.position = pos[i];
                    break;
            }
        }
    }


    /// <summary>
    /// The Magician executes the attack corresponding to her current stage
    /// </summary>
    /// 
    public override void Attack()
    {
        base.Attack();
        switch (currentStage)
        {
            case Stage.Backstage:
                break;
            case Stage.Card:
                StartCoroutine(cardStageBulletPattern.DoBulletPattern(this));

                break;
            case Stage.Dove:
                int ran = UnityEngine.Random.Range(0, 2);
                if(ran == 0)
                {
                    StartCoroutine(doveStageBulletPattern1.DoBulletPattern(this));
                }
                else
                {
                    StartCoroutine(doveStageBulletPattern2.DoBulletPattern(this));
                }

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
            case 1:
                attackRate = secondAttackRate;
                break;
            case 2:
                attackRate = thirdAttackRate;
                switch (currentStage)
                {
                    case Stage.Backstage:
                        break;
                    case Stage.Card:
                        StopCoroutine(cardStageBulletPattern.DoBulletPattern(this));

                        break;
                    case Stage.Dove:
                        StopCoroutine(doveStageBulletPattern1.DoBulletPattern(this));

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
