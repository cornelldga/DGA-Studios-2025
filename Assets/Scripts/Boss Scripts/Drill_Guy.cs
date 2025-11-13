using UnityEngine;
using Unity.Cinemachine;
using System;

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

        switch (currentState)
        {
            case State.Walking:
                UpdateWalking();
                break;
            case State.Targeting:
                UpdateTargeting();
                break;
            case State.Underground_Chase:
                UpdateChase();
                break;
            case State.Underground_Random:
                UpdateUG_R();
                break;
            case State.Throwing:
                // Handled by coroutine
                break;
            case State.Entering:
                UpdateEntering();
                break;
            case State.Exiting:
                UpdateExiting();
                break;
        }
    }

    private void UpdateWalking()
    {
        throw new NotImplementedException();
    }

    private void UpdateTargeting()
    {
        throw new NotImplementedException();
    }

    private void UpdateChase()
    {
        throw new NotImplementedException();
    }

    private void UpdateUG_R()
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
}
