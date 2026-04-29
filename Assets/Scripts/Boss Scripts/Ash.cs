using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// State machine controller for our Aura-based boss
/// </summary>

public class Ash : Boss
{
    public enum State
    {
        Wandering,
        SeedScatter,
        TurretScatter,
        MolotovAttack,
        TumbleweedSummon,
        Stomping,
        Desperation
    }
    public State currentState;

    public enum SeedAttack
    {
        XAttack,
        CrossAttack,
        DiamondAttack,
        StarAttack
    }
    private SeedAttack currentSeedPattern;

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
    [SerializeField] private int numTumbleweeds = 2; // will eventually be based on what phase it is
    [Header("Tumbleweed Spawn")]
    [SerializeField] GameObject tumbleweedPrefab;
    [SerializeField] GameObject orbitingTumbleweedPrefab;
    [Header("Tumbleweed Spawn Boundaries")]
    [SerializeField] float lowestSpawnPoint;
    [SerializeField] float highestSpawnPoint;
    [SerializeField] float leftBoundary;
    [SerializeField] float rightBoundary;
    [Header("Orbiting Tumbleweed Phases")]
    [SerializeField] int numOrbitingPhaseOne =2;
    [SerializeField] int numOrbitingPhaseTwo=3;
    [SerializeField] int numOrbitingPhaseThree=4;
    [SerializeField] private float orbitRadius = 1f;

    private GameObject[] orbitingTumbleweeds = new GameObject[5]; // size should be maximum spawned in phase 3
 
    [Space(5)]
    [Header("Stomp Settings")]
    [Tooltip("Radius around Ash to detect seeds for stomping")]
    [SerializeField] float stompRadius = 3f;

    [Space(5)]
    [Header("Stage Data")]
    [Tooltip("Circular Stages Center (UPDATE AND REMOVE SEED LOGIC IF STAGE IS NOT CIRCULAR)")]
    [SerializeField] private Vector2 stageCenter;
    [SerializeField] private float stageRadius;
    [SerializeField] private float fireRadius;

    [Space(5)]
    [Header("Seed Settings")]
    [Tooltip("Bush Seed Prefab")]
    [SerializeField] private GameObject basicSeedPrefab;
    [SerializeField] private float basicSeedArcHeight;
    [SerializeField] private float basicSeedLandTime;
    [Tooltip("Cactus Seed Prefab")]
    [SerializeField] private GameObject cactusSeedPrefab;
    [SerializeField] private float cactusSeedArcHeight;
    [SerializeField] private float cactusSeedLandTime;
    [Tooltip("Fire Flower Seed Prefab")]
    [SerializeField] private GameObject fireFlowerSeedPrefab;
    [SerializeField] private float fireSeedArcHeight;
    [SerializeField] private float fireSeedLandTime;
    [SerializeField] int seedRows;

    //molotov + bush stuff
    [Space(5)]
    [Header("Molotov Settings")]
    [SerializeField] Molotov molotov;
    [SerializeField] GameObject bushPrefab;

    private float stateTimer;
    private float tumbleweedCooldownTimer;
    private Vector2 wanderTarget;
    private SpriteRenderer sprite;
    private int scatterCount;
    private bool[] scatterTracking;

    [HideInInspector]
    public bool[] deployedSeeds;
    

    /// <summary>
    /// On start, we set the rigid body, and change its attributes. Immediately enter wandering and spawning her shield.
    /// </summary>
    public override void Start()
    {
        base.Start();
        sprite = GetComponent<SpriteRenderer>();

        currentState = State.Wandering;
        stateTimer = wanderTime;
        SetNewWanderTarget();
        tumbleweedCooldownTimer = UnityEngine.Random.Range(tumbleweedCooldownMin, tumbleweedCooldownMax);
        deployedSeeds = new bool[8];
        scatterCount = 0;
        scatterTracking = new bool[4];
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
            case State.TurretScatter:
                UpdateTurretScatter();
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

            if (seedsNearby && UnityEngine.Random.Range(0, 10) < 8)
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
    /// Handles logic for Turret scatter mode. 
    /// </summary>
    private void UpdateTurretScatter()
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
            ThrowMolotovAtBushes();
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
    /// Setting state to Turret Scatter. Uses a coroutine to perform the Turret scattering attack
    /// </summary>
    private void TransitionToTurretSeed()
    {
        currentState = State.TurretScatter;
        stateTimer = scatterTime;
        rb.linearVelocity = Vector2.zero;
        StartCoroutine(TurretSeeds());
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

        // replenish orbiting tumbleweeds
        StartCoroutine(SummonOrbitingTumbleweeds());


        float cooldownTime = UnityEngine.Random.Range(tumbleweedCooldownMin, tumbleweedCooldownMax);

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
        /*if (currentPhase == 2 && currentState != State.Desperation)
        {
            TransitionToDesperation();
            return;
        } */

        // check for all tumbleweeds destroyed

        

        int attackChoice = UnityEngine.Random.Range(0, 10);

        if (attackChoice < 4) 
        {
            if (GameObject.FindAnyObjectByType<Bush>() != null && scatterCount >= currentPhase + 1)
            {
                TransitionToMolotovAttack();
                scatterCount = 0;
                scatterTracking = new bool[4];
            }
            else
            {
                scatterCount++;
                TransitionToSeedScatter();
            }
            
        }
        else
        {
            TransitionToTurretSeed();
        }
    }

    /// <summary>
    /// Sets the new location to wander to. If there are seeds in the scene, it will target the nearest seed. Otherwise, it will pick a random point within a certain radius.
    /// </summary>
    private void SetNewWanderTarget()
    {
        GameObject[] seeds = GameObject.FindGameObjectsWithTag("Seed");
        
        if (seeds.Length == 0)
        {
            wanderTarget = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * wanderRadius;
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
        Array values = Enum.GetValues(typeof(SeedAttack));
        //UnityEngine.Random
        System.Random random = new System.Random();
        int i = 0;
        while (i == 0 || scatterTracking[(int) currentSeedPattern])
        {
            switch (currentPhase)
            {
                case 0:
                    currentSeedPattern = (SeedAttack)values.GetValue(random.Next(2)); // First 2
                    break;
                case 1:
                    currentSeedPattern = (SeedAttack)values.GetValue(random.Next(3)); // First 3
                    break;
                case 2:
                    currentSeedPattern = (SeedAttack)values.GetValue((values.Length - 3) + random.Next(3)); //Last 2
                    break;
            }
            i++;
        }
        scatterTracking[ (int) currentSeedPattern] = true;
        //currentSeedPattern = (SeedAttack)values.GetValue(random.Next(values.Length));
        Vector2 point1;
        Vector2 point2;

        switch (currentSeedPattern)
        {
            case SeedAttack.XAttack:

                point1 = new Vector2(1, 1);
                point1 = point1.normalized * stageRadius;
                seedInLine(point1, -point1, seedRows);
                point1 = (new Vector2(-1, 1)).normalized * stageRadius;
                seedInLine(point1, -point1, seedRows);

                break;
            case SeedAttack.CrossAttack:
                point1 = new Vector2(0, 1);
                point1 = point1.normalized * stageRadius;
                seedInLine(point1, -point1, seedRows);
                point1 = (new Vector2(1, 0)).normalized * stageRadius;
                seedInLine(point1, -point1, seedRows);

                break;
            case SeedAttack.DiamondAttack:
                point1 = new Vector2(0, 1);
                point1 = point1.normalized * stageRadius;
                point2 = new Vector2(1, 0);
                point2 = point2.normalized * stageRadius;
                seedInLine(point1, point2, seedRows);
                seedInLine(-point1, -point2, seedRows);
                seedInLine(point1, -point2, seedRows);
                seedInLine(-point1, point2, seedRows);
                
                break;
            case SeedAttack.StarAttack:
                point1 = new Vector2(0, 1);
                point1 = point1.normalized * stageRadius;
                point2 = new Vector2(1, 0)  ;
                point2 = point2.normalized * stageRadius;

                break;
            default:
                break;
        }


        
        yield return new WaitForSeconds(scatterTime);
    }


    //throws turret seed to one of six locations, and tranfers to scatter seed instead if there are already six deployed turrets
    private IEnumerator TurretSeeds()
    {
        bool cont = false;
        foreach (var b in deployedSeeds)
        {
            if (!b)
            {
                cont = true;
            }

        }
        if (cont)
        {
            GameObject seed;
            Seed seedScript;
            if (UnityEngine.Random.value < .5f)
            {
                seed = Instantiate(fireFlowerSeedPrefab, this.bulletOrigin.transform.position, Quaternion.identity);
                seedScript = seed.GetComponent<Seed>();
                seedScript.landingTime = fireSeedLandTime;
                seedScript.arcHeight = fireSeedArcHeight;
            }
            else 
            {
                seed = Instantiate(cactusSeedPrefab, this.bulletOrigin.transform.position, Quaternion.identity);
                seedScript = seed.GetComponent<Seed>();
                seedScript.landingTime = cactusSeedLandTime;
                seedScript.arcHeight = cactusSeedArcHeight;
            }

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
       
    
            Vector2 vec = Vector2.zero;
            int r = UnityEngine.Random.Range(0, 8);
            int pr = -1;
        // if extra time, check only false entries
        
        
            while (vec == Vector2.zero || deployedSeeds[pr])
            {
                switch (r)
                {
                    case 0:
                        vec = new Vector2(0, 1);
                        break;
                    case 1:
                        vec = new Vector2(1, 1);
                        break;
                    case 2:
                        vec = new Vector2(1, 0);
                        break;
                    case 3:
                        vec = new Vector2(1, -1);
                        break;
                    case 4:
                        vec = new Vector2(0, -1);
                        break;
                    case 5:
                        vec = new Vector2(-1, -1);
                        break;
                    case 6:
                        vec = new Vector2(-1, 0);
                        break;
                    case 7:
                        vec = new Vector2(-1, 1);
                        break;
                }
                pr = r;
                if (deployedSeeds[r])
                {
                    r = UnityEngine.Random.Range(0, 8);
                }

                //vec = new Vector2(UnityEngine.Random.Range(-1,2), UnityEngine.Random.Range(-1, 2));
            }

            deployedSeeds[r] = true;
            seedScript.locationID = r;
            seedScript.target = vec.normalized * (3 * stageRadius / 4);
        }
        else { TransitionToSeedScatter(); }
        yield return new WaitForSeconds(scatterTime);
    }



    private IEnumerator ThrowMolotovs()
    {
        // Placeholder
        yield return new WaitForSeconds(molotovTime);
    }

    // Summons 'numTumbleweeds' amount of tumbleweeds on either side of the field.
    private IEnumerator SummonTumbleweeds()
    {
        
        for (int i = 0; i < numTumbleweeds; i++) {
            Vector3 tumblePosition = new(0, 0, 0);
            if (i % 2 == 0) { // left
                tumblePosition = new Vector3(leftBoundary, UnityEngine.Random.Range(lowestSpawnPoint,highestSpawnPoint), 0);
                Instantiate(tumbleweedPrefab, tumblePosition, Quaternion.identity);
            }
            else // right
            {
                tumblePosition = new Vector3(rightBoundary,UnityEngine.Random.Range(lowestSpawnPoint, highestSpawnPoint) , 0);
                Instantiate(tumbleweedPrefab, tumblePosition, new Quaternion(0,180,0,0)); // would go in reverse direction
            }               
        }

        yield return new WaitForSeconds(tumbleweedTime);
    }


    // Summons orbiting tumbleweeds depending on phase
    private IEnumerator SummonOrbitingTumbleweeds() {
        for (int i = 0; i < orbitingTumbleweeds.Length; i++) {
            if (orbitingTumbleweeds[i] is null)
            {
                
                // set it to be an orbiting tumbleweed
                if (currentPhase == 0 && i < numOrbitingPhaseOne)
                {
                    float angle = i * Mathf.PI * 2 / numOrbitingPhaseOne;
                    Vector3 spawnPos = transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * orbitRadius;
                    orbitingTumbleweeds[i] = Instantiate(orbitingTumbleweedPrefab,spawnPos,Quaternion.identity);
                    orbitingTumbleweeds[i].transform.parent = this.transform;
                }
                else if (currentPhase == 1 && i < numOrbitingPhaseTwo)
                {

                    float angle = i * Mathf.PI * 2 / numOrbitingPhaseOne;
                    Vector3 spawnPos = transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * orbitRadius;
                    orbitingTumbleweeds[i] = Instantiate(orbitingTumbleweedPrefab, spawnPos, Quaternion.identity);
                    orbitingTumbleweeds[i].transform.parent = this.transform;
                }  
                else if (currentPhase == 2 && i < numOrbitingPhaseThree)
                {

                    float angle = i * Mathf.PI * 2 / numOrbitingPhaseOne;
                    Vector3 spawnPos = transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * orbitRadius;
                    orbitingTumbleweeds[i] = Instantiate(orbitingTumbleweedPrefab, spawnPos, Quaternion.identity);
                    orbitingTumbleweeds[i].transform.parent = this.transform;
                }
                
            }
        }
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

    //Helper method that throws seed in a line from world coord to world coord, of given thickness
    private void seedInLine(Vector2 point1, Vector2 point2, int thickness)
    {

        
        Vector2 seedStep = (point2-point1).normalized * fireRadius;
        Vector2 currentSeedLocation = point1;
        GameObject seed;
        Seed seedScript;
        Vector2 th;
        float randStep = UnityEngine.Random.value/2 +.5f;
        for (int i = 0; i <= (point2 - point1).magnitude / fireRadius; i++)
        {
            for (int t = 0- (thickness/2); t < thickness - (thickness / 2); t++)
            {
                th = Vector2.Perpendicular(seedStep).normalized * fireRadius * t;
                seed = Instantiate(basicSeedPrefab, this.bulletOrigin.transform.position, Quaternion.identity);
                seedScript = seed.GetComponent<Seed>();
                seedScript.landingTime = basicSeedLandTime;
                seedScript.arcHeight = basicSeedArcHeight;
                seedScript.target = currentSeedLocation + th*randStep;
                randStep = UnityEngine.Random.value/2 + .5f;
            }
            currentSeedLocation += seedStep;
        }
    }    
      //Throws Dynamite at the holes (phase 2)
    private void ThrowMolotovAtBushes()
    
    {
        GameObject[] bushes = GameObject.FindGameObjectsWithTag("Bush");
        //filter out bushes that are already on fire
        List<GameObject> validBushes = new List<GameObject>();
        foreach (GameObject b in bushes) {
            if (!b.GetComponent<Bush>().isFire()) validBushes.Add(b);
        }

        if (validBushes.Count>0)
        {
            int index = (int)(UnityEngine.Random.value * validBushes.Count);
            StartCoroutine(molotov.ThrowRoutine(bulletOrigin.position, validBushes[index].transform.position));
            attackCooldown = molotov.duration;
        }
    }

}