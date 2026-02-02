using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Granny : Boss
{
    public enum State
    {
        Idle, Out, Scavange
    }

    [Header("Movement Settings")]
    //Base speed when charging (regular)
    [SerializeField] float baseSpeed = 5f;
    // collision radius when charging and bouncing
    [SerializeField] float collisionRadius;
    // check distance of circle cast
    [SerializeField] float checkDistance;

    [Header("State Timing")]
    //How much time to get to pull out contracts.
    private float idleTime = 1f;
    //How long we should scavenge for contracts.
    private float scavengeTime = 1f;
    //Length of time to pull out contracts.
    private float outTime = 1f;

    [Header("Contracts Settings")]
    [Tooltip("List of bosses to spawn when Granny pulls out her contracts")]
    [SerializeField] List<GameObject> bosses = new List<GameObject>();
    private int contracts = 4;

    private float currentSpeed;
    //Time until we should change states.
    private float stateTimer;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        //currentState = State.Idle;
        stateTimer = idleTime;
        
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }

    public override void SetPhase()
    {
        
    }
}
