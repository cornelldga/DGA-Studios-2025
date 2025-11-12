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
    [SerializeField] Transform backStage;
    [SerializeField] Transform cardStage;
    [SerializeField] Transform doveStage;
    [SerializeField] Transform knifeStage;
    [SerializeField] BulletPattern cardStageBulletPattern;
    [SerializeField] BulletPattern doveStageBulletPattern;
    [SerializeField] BulletPattern knifeStageBulletPattern;

    [Tooltip("How long The Magician hides in the backstage")]
    [SerializeField] float backStageTime;

    [Tooltip("How long The Magician stays on any one Stage")]
    [SerializeField] float onStageTime;
    private float stageTimer;

    public Stage currentStage;
    [SerializeField] private Animator animator;

    public override void Start()
    {   
        base.Start();
        currentStage = Stage.Backstage;
        attackCooldown = onStageTime /3.1f;
        stageTimer = onStageTime;
    }

    public override void Update()
    {
        base.Update();
        if (currentStage != Stage.Backstage)
        {
            stageTimer -= Time.deltaTime;
            if (stageTimer <= 0)
            {
                stageTimer = backStageTime;
                currentStage = Stage.Backstage;
            }
        }
        else 
        {
            stageTimer -= Time.deltaTime;
            if (stageTimer <= 0)
            {
                stageTimer = onStageTime;
                Shuffle();
                ChooseNewStage();
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
    /// Moves The Magician to the current stage
    /// </summary>
    public void Shuffle()
    {

    }
    /// <summary>
    /// The Magician chooses to a new stage to move to and executes the corresponding attack
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
                //attackCooldown = stag`;
                isAttacking = true;
                break;
            case Stage.Card:
                StartCoroutine(cardStageBulletPattern.DoBulletPattern(this));
                attackCooldown = onStageTime / 3.1f;
                isAttacking = false;
                break;
            case Stage.Dove:
                StartCoroutine(doveStageBulletPattern.DoBulletPattern(this));
                attackCooldown = onStageTime / 3.1f;
                isAttacking = false;
                break;
            case Stage.Knife:
                StartCoroutine(knifeStageBulletPattern.DoBulletPattern(this));
                attackCooldown = onStageTime / 3.1f;
                isAttacking = false;
                break;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.CompareTag("Player");
    }

    public override void SetPhase(float healthPercent)
    {
        base.SetPhase(healthPercent);
        switch (currentPhase)
        {
            case 1:
                attackRate = 1.25f;
                break;
            case 2:
                attackRate = 1.5f;
                break;
        }
    }
}
