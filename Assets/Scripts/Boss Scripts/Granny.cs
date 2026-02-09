using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Granny : Boss
{
    public enum State
    {
        Idle, HoldingContract, ContractDropped, Scavange
    }
    private State currentState;

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
    // Contracts currently dropped by Granny
    public List<GameObject> currentDroppedContracts;
    //Time until we should change states.
    private float stateTimer;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sprite;

    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        currentState = State.Idle;
        stateTimer = idleTime;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        stateTimer -= Time.deltaTime;

        switch (currentState)
        {
            case State.Idle:
                //UpdateIdle();
                break;
            case State.HoldingContract:
                //UpdateHoldingContract();
                break;
            case State.ContractDropped:
                //UpdateContractDropped();
                break;
            case State.Scavange:
                UpdateScavenge();
                break;
        }

    }

    private void TransitionToIdle()
    {
        currentState = State.Idle;
        rb.linearVelocity = Vector2.zero;
    }

    private void UpdateScavenge()
    {
        GameObject nearestContract = null;
        foreach (GameObject contract in currentDroppedContracts)
        {
            if (nearestContract == null || Vector2.Distance(rb.position, contract.transform.position)
                < Vector2.Distance(rb.position, nearestContract.transform.position))
            {
                nearestContract = contract;
            }
        }
        Vector2 contractPosition = nearestContract.transform.position;
        rb.linearVelocity = (contractPosition - rb.position).normalized * baseSpeed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision)
        {
            if (currentState == State.Scavange && currentDroppedContracts.Contains(collision.gameObject))
            {
                Destroy(collision.gameObject); // TODO Handle contract disappearance, destroy for now
                currentDroppedContracts.Remove(collision.gameObject);
                if (currentDroppedContracts.Count <= 0)
                {
                    // TODO Handle phase change
                    TransitionToIdle();
                }
            }
        }
    }

    public override void SetPhase()
    {

    }
}
