using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Threading;

public class Granny : Boss
{
    public enum State
    {
        Idle, Invincible, HoldingContract, ContractDropped, Scavange, Returning
    }
    private State currentState;

    GrannyPhase2 grannyPhase2;

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
    public List<GameObject> currentDroppedContracts;
    private float contractSize = 0.5f;

    public bool contractDestroyed = false;
    private int initialBossCount;
    private bool singleContract = false;
    private bool doubleContract = false;

    [Header("Attack Settings")]
    [Tooltip("Attack Pattern for Granny when out of contract mode")]
    [SerializeField] BulletPattern coinAttack;
    [Tooltip("Attack Pattern for Granny while holding a contract")]
    [SerializeField] BulletPattern contractCoinAttack;
    [Tooltip("Attack Pattern for Granny while holding two contracts")]
    [SerializeField] BulletPattern doubleContractCoinAttack;

    [Header("Return Settings")]
    [Tooltip("Time to return to starting point")]
    [SerializeField] private float returnTime = 4f;

    [Header("Visuals")]
    [Tooltip("Shield GameObject shown while Granny is invincible")]
    [SerializeField] private GameObject shieldVisual;

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
        grannyPhase2 = GetComponent<GrannyPhase2>();
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

        ResetAnimationBools();
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
        if (shieldVisual != null) shieldVisual.SetActive(true);
    }

    private void TransitionToHoldingContract()
    {
        currentState = State.HoldingContract;
        stateTimer = outTime;
        if (shieldVisual != null) shieldVisual.SetActive(false);
    }
    public void TransitionToReturning()
    {
        // Speed to reach distance in time is dist/time
        currentSpeed = (rb.position - startingPoint).magnitude / returnTime;
        currentState = State.Returning;

        animator.SetBool("isWalking", true);
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
        if (bossActive) return;

        if (bosses.Count == 0 && availableBosses.Count == 0)
        {
            gameObject.GetComponent<GrannyPhase2>().enabled = true;
            gameObject.GetComponent<Granny>().enabled = false;
            return;
        }

        if (bosses.Count == initialBossCount / 2)
        {
            doubleContract = true;
            // animator thing here
        }
        else if (bosses.Count > 0)
        {
            singleContract = true;
            animator.SetBool("isSingle", true);
        }
        return;
    }

    /// <summary>
    /// Summons a boss for Granny
    /// Summons two bosses when Granny's contract list is halved
    /// </summary>
    public void SummonBoss()
    {
        if (bossActive) return;
        int numOfBosses = doubleContract ? 2 : 1;

        for (int i = 0; i < numOfBosses; i++)
        {
            GameObject boss = Instantiate(bosses[i]);
            Transform bossCanvas = boss.transform.Find("Boss Canvas");
            if (bossCanvas != null) bossCanvas.gameObject.SetActive(false);
            boss.GetComponent<Boss>().isInvulnerable = true;
            boss.GetComponent<Boss>().isSummoned = true;

            availableBosses.Add(boss);
            bosses.Remove(bosses[i]);
        }
        bossActive = true;
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
            // animator thing here
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
        newContract.transform.localScale = new Vector3(contractSize, contractSize, contractSize);

        Contract contractScript = newContract.GetComponent<Contract>();
        contractScript.boss = bossType;
        contractScript.granny = this;

        currentDroppedContracts.Add(newContract);
        TransitionToContractDropped();
    }

    /// <summary>
    /// Resets all the animation booleans for Granny
    /// </summary>
    private void ResetAnimationBools()
    {
        animator.SetBool("isSingle", false);
    }

    /// <summary>
    /// When contract dies, granny takes 1/4 of health
    /// </summary>
    public void TakeDamageFromContract()
    {
        base.TakeDamage(1);
    }

    public override void Attack()
    {
        BulletPattern pattern;
        if (currentState == State.HoldingContract)
        {
            pattern = doubleContract ? doubleContractCoinAttack : contractCoinAttack;
        }
        else
        {
            pattern = coinAttack;
        }

        if (pattern == null)
        {
            SetAttackCooldown(0.5f);
            return;
        }

        base.Attack();
        StartCoroutine(pattern.DoBulletPattern(this));
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
    }

    public override void SetPhase()
    {
        base.SetPhase();
        if (currentPhase == 1 && !doubleContract)
        {
            doubleContract = true;
        }

    }

    public override void Defeat()
    {
        grannyPhase2.enabled = true;
        Destroy(this);
    }
}
