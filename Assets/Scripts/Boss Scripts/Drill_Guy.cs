using UnityEngine;
using Unity.Cinemachine;
using System;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class Drill_Guy : Boss
{
    public enum State
    {
        Walking, Targeting, Underground_Chase, Underground_Random, Throwing, Entering, Exiting
    }
    public State currentState;
    [Header("State Timing")]
    //How much time to get a lock on player.
    private float targetingTime = 1f;
    //Time until we should change states.
    private float stateTimer;
    private Rigidbody2D rb;
    private CinemachineImpulseSource impulseSource;
    private List<GameObject> holes; //holes
    [SerializeField] DynamitePattern dynamitePattern;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        currentState = State.Targeting;
        stateTimer = targetingTime;
    }

    /// <summary>
    /// Updating of the statemachine.
    /// </summary>
    public override void Update()
    {
        base.Update();

        stateTimer -= Time.deltaTime;
        attackCooldown -= Time.deltaTime * attackRate;

        switch (currentState)
        {
            case State.Walking:
                UpdateWalking();
                break;
            case State.Targeting:
                UpdateTargeting();
                break;
            case State.Underground_Chase:
                UpdateUG_Chase();
                break;
            case State.Underground_Random:
                UpdateUG_Random();
                break;
            case State.Throwing:
                if (attackCooldown <= 0)
                {
                    ThrowDynamiteAtPlayer(); //phase 1
                    // ThrowDynamiteAtHoles(); //phase 2
                    attackCooldown = dynamitePattern.cooldown;
                }
                break;
            case State.Entering:
                UpdateEntering();
                break;
            case State.Exiting:
                UpdateExiting();
                break;
        }
    }


    //Throws Dynamite at the player (phase 1)
    private void ThrowDynamiteAtPlayer()
    {
        StartCoroutine(dynamitePattern.ThrowRoutine(bulletOrigin.position, GameManager.Instance.player.transform.position));
    }

     //Throws Dynamite at the holes (phase 2)
    private void ThrowDynamiteAtHoles()
    {
        foreach(GameObject hole in holes)
            StartCoroutine(dynamitePattern.ThrowRoutine(bulletOrigin.position, hole.transform.position));
        currentState = State.Targeting;
    }

    private void UpdateWalking()
    {
        throw new NotImplementedException();
    }

    private void UpdateTargeting()
    {
        throw new NotImplementedException();
    }

    private void UpdateUG_Chase()
    {
        throw new NotImplementedException();
    }

    private void UpdateUG_Random()
    {
        throw new NotImplementedException();
    }

    private void UpdateEntering()
    {
        throw new NotImplementedException();
    }

    private void UpdateExiting()
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Dynamite throwing.
    /// </summary>
    private void PerformThrow()
    {
        for (int i = 0; i<holes.Count; i++){
            Vector2 target = holes[i].transform.position;
            //Throw dynamite in that direction.
        }
    }

    public override void SetPhase()
    {
        throw new NotImplementedException();
    }
}
