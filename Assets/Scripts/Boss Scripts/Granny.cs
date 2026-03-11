using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Granny : Boss
{
    public enum State
    {
        Idle, Invincible, HoldingContract, ContractDropped, Scavange, Returning
    }
    [SerializeField] private State currentState; //TODO  de cereal

    [Header("Movement Settings")]
    //Base time to reach target while charging (regular)
    [SerializeField] float baseTime;
    // collision radius when charging and bouncing
    [SerializeField] float collisionRadius;
    // check distance of circle cast
    [SerializeField] float checkDistance;

    [Header("State Timing")]
    //How much time to get to pull out contracts.
    [SerializeField] private float idleTime;
    //How long we should scavenge for contracts.
    [SerializeField] private float scavengeTime;
    //Length of time to pull out contracts.
    [SerializeField] private float outTime;
    //Length of time to stay invincble after pulling out contract
    [SerializeField] private float invincibleTime;
    [SerializeField] private float droppedTime;


    [Header("Contracts Settings")]
    [Tooltip("List of bosses to spawn when Granny pulls out her contracts")]
    [SerializeField] List<GameObject> bosses = new List<GameObject>();
    [Tooltip("Spawn bounds, from bottom left to top right")]
    [SerializeField] List<Vector2> contractSpawnBounds = new List<Vector2>();
    [SerializeField] public List<GameObject> availableBosses = new List<GameObject>();
    public bool bossActive;
    [Tooltip("Contract prefab to instantiate")]
    [SerializeField] GameObject contractTemplate;
    // Contracts currently dropped by Granny
    public List<GameObject> currentDroppedContracts;

    private int initialBossCount;

    [Header("Return Settings")]
    [Tooltip("Time to return to starting point")]
    [SerializeField] private float returnTime = 4f;
    [Tooltip("Distance threshold to consider pig has arrived at starting point")]
    [SerializeField] private float arrivalThreshold = 0.1f;

    //Time until we should change states.
    private float stateTimer;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sprite;
    private Vector2 startingPoint;
    private float currentSpeed;
    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        initialBossCount = bosses.Count;
        currentState = State.Idle;
        stateTimer = idleTime;

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
            case State.Returning:
                UpdateReturning();
                break;
            case State.HoldingContract:
                UpdateHoldingContract();
                break;
            case State.Invincible:
                UpdateInvincible();
                break;
            case State.ContractDropped:
                UpdateContractDropped();
                break;
            case State.Scavange:
                UpdateScavenge();
                break;
        }

    }

    public void TransitionToIdle()
    {
        currentState = State.Idle;
        rb.linearVelocity = Vector2.zero;
    }

    private void UpdateIdle()
    {
        if (stateTimer <= 0)
        {
            TransitionToInvincible();
        }
    }

    private void TransitionToInvincible()
    {
        currentState = State.Invincible;
        stateTimer = invincibleTime;
        EnableRandomBosses();
        // TODO replace with animation, color for temp for now
        sprite.color = Color.purple;
    }

    private void TransitionToHoldingContract()
    {
        currentState = State.HoldingContract;
        stateTimer = outTime;
        // TODO replace with animation, color for temp for now
        sprite.color = Color.white;
    }
    public void TransitionToReturning()
    {
        // Speed to reach distance in time is dist/time
        currentSpeed = (rb.position - startingPoint).magnitude / returnTime;
        currentState = State.Returning;
    }

    private void UpdateInvincible()
    {
        if (stateTimer <= 0)
        {
            TransitionToHoldingContract();
        }
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
        if (bossActive) return;

        if (bosses.Count == 0 && availableBosses.Count == 0)
        {
            Debug.Log("Contracts Done!");
            gameObject.GetComponent<GrannyPhase2>().enabled = true;
            gameObject.GetComponent<Granny>().enabled = false;
            return;
        }

        if (bosses.Count ==  initialBossCount / 2 )
        {
            Debug.Log("Double Contract"); 
            bossActive = true;
            for (int i = 1; i >= 0; i--)
            {
                bosses[i].SetActive(true);
                availableBosses.Add(bosses[i]);
                bosses.Remove(bosses[i]);
            }
        }
        else if (bosses.Count > 0)
        {
            int index = Random.Range(0, bosses.Count);
            bosses[index].SetActive(true);
            bossActive = true;
            availableBosses.Add(bosses[index]);
            bosses.Remove(bosses[index]);
        }
        return; 
    }

    private void TransitionToContractDropped()
    {
        stateTimer = droppedTime;
        currentState = State.ContractDropped;
    }

    private void UpdateContractDropped()
    {
        if (stateTimer <= 0)
        {
            TransitionToScavange();
        }
    }

    private void TransitionToScavange()
    {
        GameObject nearestContract = FindNearestContract();
        // Speed to reach distance in time is dist/time
        currentSpeed = (rb.position - new Vector2(nearestContract.transform.position.x, nearestContract.transform.position.y)).magnitude / baseTime;
        stateTimer = scavengeTime;
        currentState = State.Scavange;
    }

    private void UpdateScavenge()
    {
        GameObject nearestContract = FindNearestContract();
        if (nearestContract != null)
        {
            Vector2 contractPosition = nearestContract.transform.position;
            Vector2 dist = contractPosition - rb.position;
            rb.linearVelocity = dist.normalized * currentSpeed;
        }
        else
        {
            currentState = State.Returning;
        }
    }

    /// <summary>
    /// Finds the nearest currently dropped contract by distance, null if none.
    /// </summary>
    /// <returns>Contract GameObject or null if no dropped contracts</returns>
    private GameObject FindNearestContract()
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
        return nearestContract;
    }

    private void UpdateReturning()
    {
        Vector2 dist = startingPoint - rb.position;
        if (dist.magnitude < 0.1)
        {
            TransitionToIdle();
        }
        else
        {
            rb.position = new Vector3(rb.position.x, rb.position.y, -1);
            rb.linearVelocity = dist.normalized * currentSpeed;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision)
        {
            if (currentState == State.Scavange && currentDroppedContracts.Contains(collision.gameObject))
            {
                currentDroppedContracts.Remove(collision.gameObject);
                Destroy(collision.gameObject); // TODO Handle contract disappearance, destroy for now
                if (currentDroppedContracts.Count <= 0)
                {
                    // TODO Handle phase change
                    TransitionToReturning();
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
        Vector3 randomPos = new Vector3(Random.Range(contractSpawnBounds[0].x, contractSpawnBounds[1].x),
            Random.Range(contractSpawnBounds[0].y, contractSpawnBounds[1].y), -1);
        GameObject newContract = Instantiate(contractTemplate, randomPos, Quaternion.identity);

        Contract contractScript = newContract.GetComponent<Contract>();
        contractScript.boss = bossType;
        contractScript.granny = this;

        currentDroppedContracts.Add(newContract);
        TransitionToContractDropped();
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
        }
        else
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
