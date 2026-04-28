using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Granny : Boss
{
    public enum State
    {
        Idle, Invincible, HoldingContract, ContractDropped, Scavange, Returning
    }
    private State currentState;

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

    public bool contractDestroyed = false;
    private int initialBossCount;
    private bool doubleContract = false;

    [Header("Return Settings")]
    [Tooltip("Time to return to starting point")]
    [SerializeField] private float returnTime = 4f;

    //Time until we should change states.
    private float stateTimer;
    private SpriteRenderer sprite;
    private Vector2 startingPoint;
    private float currentSpeed;

    public override void Start()
    {
        base.Start();
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
        stateTimer = idleTime;
        rb.linearVelocity = Vector2.zero;

        animator.SetBool("isWalking", false);
        animator.SetBool("isSingleIdle", false);
        animator.SetBool("isDoubleIdle", false);
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
        // Purple until animation
        sprite.color = Color.purple;
    }

    private void TransitionToHoldingContract()
    {
        currentState = State.HoldingContract;
        stateTimer = outTime;
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

    /// <summary>
    /// Enables random bosses. Enables the last two bosses when contract list is down to 2.
    /// </summary>
    private void EnableRandomBosses()
    {
        animator.SetBool("isHit", false);
        if (bossActive) return;

        if (bosses.Count == 0 && availableBosses.Count == 0)
        {
            gameObject.GetComponent<GrannyPhase2>().enabled = true;
            gameObject.GetComponent<Granny>().enabled = false;
            return;
        }

        if (bosses.Count == initialBossCount / 2)
        {
            animator.SetBool("isDouble", true);
        }
        else if (bosses.Count > 0)
        {
            animator.SetBool("isSingle", true);
        }
        return;
    }

    public void SummonBoss()
    {
        if (bossActive) return;
        int index = Random.Range(0, bosses.Count);
        GameObject boss = Instantiate(bosses[index]);
        Transform bossCanvas = boss.transform.Find("Boss Canvas");
        if (bossCanvas != null) bossCanvas.gameObject.SetActive(false);
        boss.GetComponent<Boss>().isInvulnerable = true;

        availableBosses.Add(boss);
        bosses.Remove(bosses[index]);
        bossActive = true;
    }

    public void SummonBosses()
    {
        if (bossActive) return;
        for (int i = 1; i >= 0; i--)
        {
            GameObject boss = Instantiate(bosses[i]);
            Transform bossCanvas = boss.transform.Find("Boss Canvas");
            if (bossCanvas != null) bossCanvas.gameObject.SetActive(false);
            boss.GetComponent<Boss>().isInvulnerable = true;

            availableBosses.Add(boss);
            bosses.Remove(bosses[i]);
        }
        bossActive = true;
        doubleContract = true;
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

        animator.SetBool("isWalking", true);
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
                Destroy(collision.gameObject);
                if (currentDroppedContracts.Count <= 0)
                {
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
    /// When contract dies, granny takes 1/4 of health
    /// </summary>
    public void TakeDamageFromContract()
    {
        base.TakeDamage(1);
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

        if (availableBosses == null)
        {
            return;
        }
        int index = Random.Range(0, availableBosses.Count);
        DropNewContract(availableBosses[index]);

        animator.SetBool("isHit", true);
    }

    public override void Attack()
    {
        // Granny does not shoot — skip base bullet logic
    }

    public override void SetPhase()
    {
        if (currentPhase == 1 && !doubleContract)
        {
            doubleContract = true;
        }
        healthBarAnimator.SetTrigger("PhaseChange");
    }
}
