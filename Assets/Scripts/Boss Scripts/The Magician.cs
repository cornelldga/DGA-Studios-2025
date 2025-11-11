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
    [SerializeField] BulletPattern DesperationAttack;
    [Tooltip("How long The Magician hides in the backstage")]
    [SerializeField] float backStageTime;

    public Stage currentStage;
    [SerializeField] private Animator animator;

    public override void Start()
    {   
        base.Start();
        currentStage = Stage.Backstage;
        attackCooldown = backStageTime;
    }

    public override void Update()
    {
        base.Update();
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
    /// Choose a random stage location that was not chosen previousely
    /// </summary>
    private void ChooseNewStage()
    {
        List<Stage> stages = new List<Stage>();
        stages.AddRange(Enum.GetValues(typeof(Stage)));
        stages.Remove(currentStage);
        currentStage = stages[UnityEngine.Random.Range(0, stages.Count)];
    }
    /// <summary>
    /// The Magician chooses to a new stage to move to and executes the corresponding attack
    /// </summary>
    public override void Attack()
    {
        ChooseNewStage();
        MoveToStage();
        base.Attack();
        switch (currentStage)
        {
            case Stage.Backstage:
                attackCooldown = backStageTime;
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
