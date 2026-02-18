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
    [SerializeField] private float idleTime = 1f;
    //How long we should scavenge for contracts.
    [SerializeField] private float scavengeTime = 1f;
    //Length of time to pull out contracts.
    [SerializeField] private float outTime = 1f;
    [SerializeField] private float droppedTime = 1f;


    [Header("Contracts Settings")]
    [Tooltip("List of bosses to spawn when Granny pulls out her contracts")]
    [SerializeField] List<GameObject> bosses = new List<GameObject>();
    [Tooltip("Spawn bounds, from bottom left to top right")]
    [SerializeField] List<Vector2> contractSpawnBounds = new List<Vector2>();
    [SerializeField] List<GameObject> availableBosses = new List<GameObject>();
    [Tooltip("Contract prefab to instantiate")]
    [SerializeField] GameObject contractTemplate;
    // Contracts currently dropped by Granny
    public List<GameObject> currentDroppedContracts;

    private int initialBossCount;

    [Header("Return Settings")]
    [Tooltip("Speed when returning to starting point")]
    [SerializeField] private float returnSpeed = 4f;
    [Tooltip("Distance threshold to consider pig has arrived at starting point")]
    [SerializeField] private float arrivalThreshold = 0.1f;

    private float currentSpeed;
    //Time until we should change states.
    private float stateTimer;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sprite;
    private Vector2 startingPoint;

    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        initialBossCount = bosses.Count;
        currentState = State.Idle;
        stateTimer = idleTime;
        currentSpeed = baseSpeed;

        startingPoint = new Vector2(transform.position.x, transform.position.y);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        stateTimer -= Time.deltaTime;

        switch (currentState)
        {
            case State.Idle:
                UpdateIdle();
                break;
            case State.HoldingContract:
                UpdateHoldingContract();
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
        if (rb == null) return;

        Vector2 directionToStart = (startingPoint - (Vector2)transform.position).normalized;

        rb.linearVelocity = directionToStart * returnSpeed;

        float distanceToStart = Vector2.Distance(transform.position, startingPoint);
        if (distanceToStart <= arrivalThreshold)
        {
            rb.linearVelocity = Vector2.zero;
            transform.position = startingPoint;
        }

        currentState = State.Idle;
        rb.linearVelocity = Vector2.zero;
    }

    private void UpdateIdle()
    {
        if (stateTimer <= 0)
        {
            TransitionToHoldingContract();
        }
    }

    private void TransitionToHoldingContract()
    {
        currentState = State.HoldingContract;
        stateTimer = outTime;
    }

    private void UpdateHoldingContract()
    {
        if (stateTimer <= 0)
        {
            EnableRandomBosses();
        }
    }

    private void EnableRandomBosses()
    {
        if (bosses.Exists(b => b.activeInHierarchy)) return;

        if (bosses.Count == 0) return; //TODO Phase 2 Switch

        if (bosses.Count <= initialBossCount / 2)
        {
            for (int i = 0; i < bosses.Count; i++)
            {
                int index = Random.Range(0, bosses.Count);
                bosses[index].SetActive(true);
                bosses.RemoveAt(index);
            }
        }
        else
        {
            int index = Random.Range(0, bosses.Count);
            bosses[index].SetActive(true);
            availableBosses.Add(bosses[index]);
        }
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


    /// <summary>
    /// Creates a new contract, connects it with the boss, 
    /// </summary>
    /// <param name="bossType"></param>
    private void DropNewContract(GameObject bossType)
    {
        Vector2 randomPos = new Vector2(Random.Range(contractSpawnBounds[0].x, contractSpawnBounds[1].x),
            Random.Range(contractSpawnBounds[0].y, contractSpawnBounds[1].y));
        GameObject newContract = Instantiate(contractTemplate, randomPos, Quaternion.identity);

        Contract contractScript = newContract.GetComponent<Contract>();
        contractScript.boss = bossType;
        contractScript.granny = this;

        currentDroppedContracts.Add(newContract);
    }

    /// <summary>
    /// Drops contract when taking any damage instead of dying
    /// </summary>
    /// <param name="damage">Only drops contract if > 0</param>
    public override void TakeDamage(float damage)
    {
        if (damage <= 0 || currentState != State.HoldingContract)
        {
            return;
        } else
        {
            if (availableBosses == null)
            {
                return;
            }
            int index = Random.Range(0, availableBosses.Count);
            DropNewContract(availableBosses[index]);
        }
    }

    public override void Attack()
    {
        // Granny does not shoot — skip base bullet logic
    }

    public override void SetPhase()
    {
        // Could maube put when contracts are zero here but probably not
    }
}
