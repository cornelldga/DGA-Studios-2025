using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

/// <summary>
/// State machine controller for our Aura-based boss
/// </summary>

public class Ash : Boss
{
    public enum State
    {
        Wandering,
        SeedScatter,
        MolotovAttack,
        TumbleweedSummon,
        Stomping,
        Desperation
    }
    public State currentState;

    [Header("Movement Settings")]
    [SerializeField] float wanderSpeed = 2f;
    [SerializeField] float wanderRadius = 5f;

    [Space(5)]
    [Header("State Timing")]
    [SerializeField] float wanderTime = 3f;
    [SerializeField] float scatterTime = 1.5f;
    [SerializeField] float molotovTime = 2f;
    [SerializeField] float tumbleweedTime = 2f;
    [Tooltip("Duration of stomp animation that grows flowers")]
    [SerializeField] float stompTime = 1.5f;

    [Space(5)]
    [Header("Tumbleweed Summon Timing")]
    [SerializeField] float tumbleweedCooldownMin = 10f;
    [SerializeField] float tumbleweedCooldownMax = 15f;

    [Space(5)]
    [Header("Stomp Settings")]
    [Tooltip("Radius around Ash to detect seeds for stomping")]
    [SerializeField] float stompRadius = 3f;

    private float stateTimer;
    private float tumbleweedCooldownTimer;
    private Rigidbody2D rb;
    private Vector2 wanderTarget;
    private Animator animator;
    private SpriteRenderer sprite;

    /// <summary>
    /// On start, we set the rigid body, and change its attributes. Immediately enter wandering and spawning her shield.
    /// </summary>
    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        currentState = State.Wandering;
        stateTimer = wanderTime;
        SetNewWanderTarget();
        tumbleweedCooldownTimer = Random.Range(tumbleweedCooldownMin, tumbleweedCooldownMax);
    }

    /// <summary>
    /// Updating of the statemachine.
    /// </summary>
    public override void Update()
    {
        base.Update();
        stateTimer -= Time.deltaTime;
        tumbleweedCooldownTimer -= Time.deltaTime;
        animator.SetBool("isWalking", rb.linearVelocity.magnitude > 0.1f);

        if (tumbleweedCooldownTimer <= 0f && currentState != State.Desperation)
        {
            if (currentState == State.Wandering)
            {
                TransitionToTumbleweedSummon();
            }
        }

        switch (currentState)
        {
            case State.Wandering:
                UpdateWandering();
                break;
            case State.SeedScatter:
                UpdateSeedScatter();
                break;
            case State.MolotovAttack:
                UpdateMolotovAttack();
                break;
            case State.TumbleweedSummon:
                UpdateTumbleweedSummon();
                break;
            case State.Stomping:  
                UpdateStomping();
                break;
            case State.Desperation:
                UpdateDesperation();
                break;
        }
    }

    /// <summary>
    /// Handles logic for wandering mode. Checks for nearby seeds to stomp, otherwise moves towards a target point. If it reaches the target or the timer runs out, it chooses the next attack.
    /// </summary>
    private void UpdateWandering()
    {
        Vector2 direction = (wanderTarget - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * wanderSpeed;

        if (direction.x > 0) sprite.flipX = true;
        else if (direction.x < 0) sprite.flipX = false;

        if (Vector2.Distance(transform.position, wanderTarget) < 0.5f)
        {
            GameObject[] nearbySeeds = GameObject.FindGameObjectsWithTag("Seed");
            bool seedsNearby = false;
            foreach (GameObject seed in nearbySeeds)
            {
                if (Vector2.Distance(transform.position, seed.transform.position) <= stompRadius)
                {
                    seedsNearby = true;
                    break;
                }
            }

            if (seedsNearby)
                TransitionToStomping();
            else
                ChooseNextAttack(); 
        }
        else if (stateTimer <= 0)
        {
            ChooseNextAttack(); 
        }
    }

    /// <summary>
    /// Handles logic for seed scatter mode.
    /// </summary>
    private void UpdateSeedScatter()
    {
        rb.linearVelocity = Vector2.zero;
        if (stateTimer <= 0)
        {
            TransitionToWandering();
        }
    }

    /// <summary>
    /// Handles logic for molotov attack mode.
    /// </summary>
    private void UpdateMolotovAttack()
    {
        rb.linearVelocity = Vector2.zero;
        if (stateTimer <= 0)
        {
            TransitionToWandering();
        }
    }

    /// <summary>
    /// Handles logic for summoning tumbleweeds mode.
    /// </summary>
    private void UpdateTumbleweedSummon()
    {
        rb.linearVelocity = Vector2.zero;
        if (stateTimer <= 0)
        {
            TransitionToWandering();
        }
    }

    /// <summary>
    /// Handles logic for stomping mode.
    /// </summary>
    private void UpdateStomping()
    {
        rb.linearVelocity = Vector2.zero;
        if (stateTimer <= 0)
        {
            TransitionToWandering();
        }
    }

    /// <summary>
    /// Handles logic for desperation mode.
    /// </summary>
    private void UpdateDesperation()
    {
        rb.linearVelocity = Vector2.zero;
        if (stateTimer <= 0)
        {
            TransitionToWandering();
        }
    }

    /// <summary>
    /// Setting state to Wandering. Uses a coroutine to perform the wandering movement
    /// </summary>
    private void TransitionToWandering()
    {
        currentState = State.Wandering;
        stateTimer = wanderTime;
        SetNewWanderTarget();
    }

    /// <summary>
    /// Setting state to Seed Scatter. Uses a coroutine to perform the seed scattering attack
    /// </summary>
    private void TransitionToSeedScatter()
    {
        currentState = State.SeedScatter;
        stateTimer = scatterTime;
        rb.linearVelocity = Vector2.zero;
        StartCoroutine(ScatterSeeds());
    }

    /// <summary>
    /// Setting state to Molotov Attack. Uses a coroutine to perform the molotov throwing attack
    /// </summary>
    private void TransitionToMolotovAttack()
    {
        currentState = State.MolotovAttack;
        stateTimer = molotovTime;
        StartCoroutine(ThrowMolotovs());
    }

    /// <summary>
    /// Setting state to Tumbleweed Summon. Uses a coroutine to perform the tumbleweed summoning attack
    /// </summary>
    private void TransitionToTumbleweedSummon()
    {
        currentState = State.TumbleweedSummon;
        stateTimer = tumbleweedTime;

        float cooldownTime = Random.Range(tumbleweedCooldownMin, tumbleweedCooldownMax);

        if (GetHealthPercent() <= 0.2f)
        {
            cooldownTime *= 0.5f;
        }

        tumbleweedCooldownTimer = cooldownTime;

        StartCoroutine(SummonTumbleweeds());
    }


    /// <summary>
    /// Setting state to Stomping. Uses a coroutine to perform the stomping attack
    /// </summary>
    private void TransitionToStomping()
    {
        currentState = State.Stomping;
        stateTimer = stompTime;
        StartCoroutine(StompAndGrowFlowers());
    }

    /// <summary>
    /// Setting state to Desperation. Uses a coroutine to perform the desperation attack
    /// </summary>
    private void TransitionToDesperation()
    {
        currentState = State.Desperation;
        stateTimer = 5f;
        StartCoroutine(DesperationAttack());
    }

    /// <summary>
    /// Decides which attack to perform based on desperation state and random chance.
    /// 60% molotov attack, 40% seed scatter.
    /// </summary>
    private void ChooseNextAttack()
    {
        // check for desperation
        if (GetHealthPercent() <= 0.2f && currentState != State.Desperation)
        {
            TransitionToDesperation();
            return;
        }

        int attackChoice = Random.Range(0, 10);

        if (attackChoice < 4)
        {
            TransitionToSeedScatter();
        }
        else
        {
            TransitionToMolotovAttack();
        }
    }

    /// <summary>
    /// Sets the phase of the boss. Used for changing behavior based on health thresholds.
    /// </summary>
    public override void SetPhase()
    {
       // Placeholder
    }

    /// <summary>
    /// Sets the new location to wander to. If there are seeds in the scene, it will target the nearest seed. Otherwise, it will pick a random point within a certain radius.
    /// </summary>
    private void SetNewWanderTarget()
    {
        GameObject[] seeds = GameObject.FindGameObjectsWithTag("Seed");
        
        if (seeds.Length == 0)
        {
            wanderTarget = (Vector2)transform.position + Random.insideUnitCircle * wanderRadius;
            return;
        }

        GameObject nearest = null;
        float closestDist = Mathf.Infinity;

        foreach (GameObject seed in seeds)
        {
            float dist = Vector2.Distance(transform.position, seed.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                nearest = seed;
            }
        }

        wanderTarget = nearest.transform.position;
    }

    private IEnumerator ScatterSeeds()
    {
        // Placeholder
        int[,] seedField =  new int[20 , 10];
        foreach (int seed in seedField) 
        { 
            //seed
        }
        yield return new WaitForSeconds(scatterTime);
    }

    private IEnumerator ThrowMolotovs()
    {
        // Placeholder
        yield return new WaitForSeconds(molotovTime);
    }

    private IEnumerator SummonTumbleweeds()
    {
        // Placeholder
        yield return new WaitForSeconds(tumbleweedTime);
    }

    // Stomps on nearby seeds, destroying them. No animation yet. eventually will grow them into flowers.
    private IEnumerator StompAndGrowFlowers()
    {
        GameObject[] seeds = GameObject.FindGameObjectsWithTag("Seed");
        foreach (GameObject seed in seeds)
        {       
            if (Vector2.Distance(transform.position, seed.transform.position) <= stompRadius)
            {
                Seed s = seed.GetComponent<Seed>();
                s.Blossom();
            }
        }
        
        yield return new WaitForSeconds(stompTime);
    }
    private IEnumerator DesperationAttack()
    {
        // Placeholder
        yield return new WaitForSeconds(5f);
    }
    private float GetHealthPercent()
    {
        return (float)health / maxHealth;
    }

    public override void Attack()
    {
        // Uses state machine instead of base attack
    }

}