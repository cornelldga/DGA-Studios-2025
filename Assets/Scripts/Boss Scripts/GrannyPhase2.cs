using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;

public class GrannyPhase2 : Boss
{
    public enum State
    {
        Idle, Lazer, MachineGun, Punch
    }
    private State currentState;

    [Header("Movement Settings")]
    //Base speed when charging (regular)
    [SerializeField] float baseSpeed = 5f;

    private Vector2 targetPosition;

    [Header("State Timing")]
    //How much time to get to pull out contracts.
    private float idleTime = 1f;
    //How long we should scavenge for contracts.
    private float scavengeTime = 1f;
    //Length of time to pull out contracts.
    private float outTime = 1f;

    private float currentSpeed;
    //Time until we should change states.
    private float stateTimer;
    private float firingCooldown;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sprite;

    [Header("Bullet Patterns")]
    [SerializeField] BulletPattern machineGun;
    [SerializeField] BulletPattern lazerShot;

    [Header("Attack Constants")]
    [SerializeField] private float machineCooldownConstant;
    private float machineTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        currentState = State.MachineGun;
        stateTimer = idleTime;
        machineTimer = 0;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        stateTimer -= Time.deltaTime;

        switch (currentState)
        {
            case State.Idle:
                UpdateTargeting();
                break;
            case State.Lazer:
                UpdateLazer();
                break;
            case State.MachineGun: 
                UpdateMachineGun();
                break;
            case State.Punch:
                UpdatePunch();
                break;
        }
    }


    /// <summary>
    /// Handles logic for targeting mode.
    /// </summary>
    private void UpdateTargeting()
    {
        // Track the player's position
        if (GameManager.Instance != null && GameManager.Instance.player != null)
        {
            targetPosition = GameManager.Instance.player.transform.position;
        }

        // When targeting time is up, decide what to do
        if (stateTimer <= 0)
        {
            //decide which mode to swap to based on some randomization
        }
    }

    private void UpdateLazer()
    {
        //showcase the lazer tuning on and honing in on the player
        //shoot the lazer for the 1 shot hit
    }


    private void UpdateMachineGun()
    {
        if(machineTimer == 0)
        {
            //track the player location as you shoot out bullets
            //granny moves as she shoots machine gun
            bulletOrigin.transform.right = GameManager.Instance.player.transform.position
                    - bulletOrigin.transform.position;

            if (bulletOrigin.transform.right.x > 0) { sprite.flipX = true; }
            else if (bulletOrigin.right.x < 0) { sprite.flipX = false; }
            StartCoroutine(machineGun.DoBulletPattern(this));
            machineTimer += Time.deltaTime;
        } else if(machineTimer >= machineCooldownConstant)
        {
            machineTimer = 0;
        } else
        {
            machineTimer += Time.deltaTime;
        }
    }

    private void  UpdatePunch(){
        //move granny towards the player while using her punch move that is 
        //maybe make the punch a separate hitbox

    }

    public override void SetPhase()
    {
        
    }
}
