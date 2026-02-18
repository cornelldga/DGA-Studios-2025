using UnityEngine;
using Unity.Cinemachine;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Splines;
using Unity.Collections;

public class DrillGuy : Boss
{
    
    public enum State
    {
        Walking, Targeting, Underground_Chase, Underground_Random, Throwing, Entering, Exiting
    }
    public State currentState;
    [Header("State Timing")]
    //How much time to get a lock on player.
    [SerializeField] float initialThrowTime = 7.3f;
    [SerializeField] float walkingTime = 4f;
    //Time until we should change states.
    private float stateTimer;
    private Rigidbody2D rb;
    private bool isUnderground;
    private CinemachineImpulseSource impulseSource;
    private List<Vector3> holePositions = new List<Vector3>();
    [SerializeField] BulletPattern debrisPattern;
    [SerializeField] int numFrenzyDigs;

    [Header("Hole Settings")]

    [SerializeField] float moveSpeed;
    [Tooltip("The range in which the driller can move from a random point in world center")]
    [SerializeField] float moveRange;

    [Tooltip("The drill hole prefab object that is created by Drill Guy's transition from underground to aboveground")]
    [SerializeField] private GameObject exitHolePrefab;

    [Tooltip("The drill hole prefab object that is created by Drill Guy's transition from aboveground to underground")]
    [SerializeField] private GameObject enterHolePrefab;

    // want to look to disable hurtbox if underground. we also need to make Boss.TakeDamage() overridable
    private BoxCollider2D hurtBox;

    // similarly we have a different hitbox for when it is underground
    private CircleCollider2D pushTrigger;

    [Header("Dig Settings")]

    [Tooltip("Specifies the max push strength. Increase this to cause more dispalcement from the drill dig path.")]
    [SerializeField] private float pushForce = 30f;
    // the path we take in digging
    private DrillPath currentPath;
    // how far along we are along this path
    private float t;
    // how fast we should complete this dig (DO NOT GO OVER 2 ITS OP!)
    [SerializeField] float speedModifier = 0.5f;
    [SerializeField] float pushRadius = 1f;

    [Tooltip("The range in which the driller can dig from a random point in world center")]
    [SerializeField] float digRange;
    [Tooltip("Driller Animation Controller")]
    private Animator animator;

    //Time until we should change states.
    [SerializeField] Dynamite dynamite;

    [Header("Throwing Settings")]
    [SerializeField] GameObject dynamiteLandingIndicatorPrefab;
    [Tooltip("How innacurate a dynamite throw is during phase 1")]
    [SerializeField] float throw1Innacuracy;
    [Tooltip("How innacurate a dynamite throw is during phase 2")]
    [SerializeField] float throw2Innacuracy;
    [Tooltip("Percent chance that throwing will be chosen phase 1")]
    [SerializeField] float phase1ThrowChance;

    Vector3 movePosition;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        currentState = State.Walking;
        stateTimer = walkingTime;
        animator = GetComponent<Animator>();
        isUnderground = false;
        hurtBox = GetComponent<BoxCollider2D>();
        pushTrigger = GetComponent<CircleCollider2D>();
        pushTrigger.enabled = false;
    }

    /// <summary>
    /// Updating of the statemachine.
    /// </summary>
    public override void Update()
    {
        base.Update();
        stateTimer -= Time.deltaTime;
        attackCooldown -= Time.deltaTime * attackRate;

        switch (currentState)
        {
            case State.Walking:
                UpdateWalking();
                break;
            case State.Targeting:
                UpdateTargeting();
                break;
            case State.Underground_Chase:
                UpdateUG_Chase();
                break;
            case State.Underground_Random:
                UpdateUG_Random();
                break;
            case State.Throwing:
                UpdateThrowing();
                break;
            case State.Entering:
                UpdateEntering();
                break;
            case State.Exiting:
                UpdateExiting();
                break;
        }
    }

    //Throws Dynamite at the player (phase 1)
    private void ThrowDynamiteAtPlayer()
    {
        float throwInnacuracy = currentPhase == 1 ? throw1Innacuracy : throw2Innacuracy;
        Vector3 landingPos = GameManager.Instance.player.transform.position +
            new Vector3(UnityEngine.Random.Range(-throwInnacuracy, throwInnacuracy),
            UnityEngine.Random.Range(-throwInnacuracy, throwInnacuracy));
        GameObject landingIndicator = Instantiate(dynamiteLandingIndicatorPrefab, landingPos, Quaternion.identity);
        Destroy(landingIndicator, dynamite.duration);
        StartCoroutine(dynamite.ThrowRoutine(bulletOrigin.position, landingPos));
        attackCooldown = dynamite.duration;
    }

     //Throws Dynamite at the holes (phase 2)
    private void ThrowDynamiteAtHoles()
    {
        foreach(Vector3 pos in holePositions)
        {
            GameObject landingIndicator = Instantiate(dynamiteLandingIndicatorPrefab, pos, Quaternion.identity);
            Destroy(landingIndicator, dynamite.duration);
            StartCoroutine(dynamite.ThrowRoutine(bulletOrigin.position, pos));
        }
        attackCooldown = dynamite.duration;
        holePositions.Clear();
    }

    /// <summary>
    /// Transition to walking.
    /// </summary>
    private void TransitionToWalking()
    {
        ResetAllAnimatorBools();
        animator.SetBool("isWalking", true);
        currentState = State.Walking;
        stateTimer = walkingTime;
        movePosition =
            new Vector3(UnityEngine.Random.Range(-moveRange, moveRange),
            UnityEngine.Random.Range(-moveRange, moveRange));
    }

    /// <summary>
    /// helper function to face player lolz
    /// </summary>
    private void FacePlayer()
    {
        Vector2 playerPos = GameManager.Instance.player.transform.position;
        Vector2 currentPos = transform.position;
        Vector2 direction = (playerPos - currentPos).normalized;
        
        if (direction.x < 0)
        {
            transform.localScale = new Vector3(1, 1, 1); 
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1); 
        }
    }

    /// <summary>
    /// What the boss does when walking around
    /// Planning for attacking the player is done in Targeting
    /// </summary>
    private void UpdateWalking()
    {
        FacePlayer();
        if (stateTimer <= 0)
        {
            TransitionToEntering();
        }
        else
        {
            Vector3 moveVector = movePosition - transform.position;
            if(moveVector.magnitude <= .1f)
            {
                movePosition =
                new Vector3(UnityEngine.Random.Range(-moveRange, moveRange),
                UnityEngine.Random.Range(-moveRange, moveRange));
            }
            moveVector = moveVector.normalized * moveSpeed;
            rb.linearVelocity = new Vector2(moveVector.x, moveVector.y);
        }
    }

    /// <summary>
    /// Transition to throwing state
    /// </summary>
    private void TransitionToThrowing()
    {
        ResetAllAnimatorBools();
        animator.SetBool("isThrowing", true);
        currentState = State.Throwing;
        stateTimer = initialThrowTime;
    }

    /// <summary>
    /// Updates actions for the drill during throw state
    /// </summary>
    private void UpdateThrowing()
    {
        FacePlayer();
        if (stateTimer <= 0)
        {
            TransitionToWalking();
        }
        else if (attackCooldown <= 0)
        {
            if (currentPhase == 0)
            {
                    ThrowDynamiteAtHoles(); //phase 1
                    TransitionToWalking();
            }
            else if (currentPhase == 1) {
                float ran = UnityEngine.Random.Range(0, 1f);
                if(ran <= phase1ThrowChance)
                {
                    ThrowDynamiteAtPlayer(); //phase 2                
                }
                else
                {
                    ThrowDynamiteAtHoles(); //phase 1
                }
            }
            else
            {
                if (numFrenzyDigs == 0)
                {
                    // BOOM!
                    numFrenzyDigs = 3;
                    ThrowDynamiteAtPlayer();
                    ThrowDynamiteAtPlayer();
                    ThrowDynamiteAtPlayer();
                    ThrowDynamiteAtPlayer();
                    ThrowDynamiteAtPlayer();
                    ThrowDynamiteAtHoles();
                }
            }
        }
    }

    private void UpdateTargeting()
    {
        throw new NotImplementedException();
    }

    private void TransitionToUGChase()
    {
        ResetAllAnimatorBools();
        animator.SetBool("isUG", true);
        currentState = State.Underground_Chase;
        CreateChasePathToPlayer();
        StartCoroutine(DigPath());
    }

    private void UpdateUG_Chase()
    {
        if (t >= 1f)
        {
            TransitionToExiting();
        }
    }
    private void TransitionToUGRandom()
    {
        ResetAllAnimatorBools();
        animator.SetBool("isUG", true);
        currentState = State.Underground_Random;
        // no need to check frenzy digs here, we assume that if we got to this state that has already been =
        // taken care of
        CreateChasePathToRandom();
        StartCoroutine(DigPath());
    }

    private void UpdateUG_Random()
    {
        if (t >= 1f)
        {
            TransitionToExiting();
        }

    }

    /// <summary>
    /// Transitions to the Entering state
    /// </summary>
    private void TransitionToEntering()
    {
        rb.linearVelocity = Vector2.zero;
        ResetAllAnimatorBools();
        animator.SetBool("isEntering", true);
        currentState = State.Entering;
    }

    /// <summary>
    /// Updates the Drill boss during Entering state.
    /// </summary>
    private void UpdateEntering()
    {

    }

    /// <summary>
    /// Animation triggered event to spawn drill hole to match up with animation
    /// </summary>
    public void AnimationOnEnteredGround()
    {
        Vector3 spawnPos = bulletOrigin.position;
        Instantiate(enterHolePrefab, spawnPos, Quaternion.identity);
        holePositions.Add(spawnPos);
        pushTrigger.enabled = true;
        hurtBox.enabled = false;
        isUnderground = true;
    }

    /// <summary>
    /// Drill entering finish Animation event, transitions to UG
    /// </summary>
    public void AnimationOnEnteringFinished()
    {
        if (currentState == State.Entering && currentPhase < 2)
            TransitionToUGChase();
        else
            TransitionToUGRandom();

    }

    /// <summary>
    /// Transitions to exiting state
    /// </summary>
    private void TransitionToExiting()
    {
        isUnderground = false;
        ResetAllAnimatorBools();
        animator.SetBool("isExiting", true);
        currentState = State.Exiting;

        Vector3 spawnPos = transform.position;
        StartCoroutine(debrisPattern.DoBulletPattern(this));
        Instantiate(exitHolePrefab, spawnPos, Quaternion.identity);
        holePositions.Add(spawnPos);
    }

    /// <summary>
    /// Updates the Drill boss during Exiting state.
    /// </summary>
    private void UpdateExiting()
    {
        
    }

    /// <summary>
    /// called when exiting animation finishes, lets state machine know boss can now move on
    /// </summary>
    public void AnimationOnExitingFinished()
    {
        hurtBox.enabled = true;
        pushTrigger.enabled = false;
        if (currentState == State.Exiting)
        {
            if (currentPhase == 2 && numFrenzyDigs > 0){
                // if it's desperation phase and there's more to dig
                numFrenzyDigs -= 1;
                TransitionToEntering();
            }
            else{
                // ideally there is a delay before first throw, or else weird stuff can happen (animation)
                // also our current throw function its really messy can get kinda broken in animations
                // if not careful
                TransitionToThrowing();
            }
        }
    }

    /// <summary>
    /// Generates a cubic bezier path from current position to a little bit player position
    /// Prob should be called in target?
    /// p1 and p2 are random as of now.
    /// </summary>
    private void CreateChasePathToPlayer()
    {
        Vector2 p0 = transform.position; 
        Vector2 p3 = GameManager.Instance.player.transform.position; 
        
        Vector2 directionToPlayer = (p3 - p0).normalized;
        float distanceToPlayer = Vector2.Distance(p0, p3);
        
        Vector2 p1 = GenerateControlPoint(p0, p3, directionToPlayer, distanceToPlayer, 0.15f);
        
        Vector2 p2 = GenerateControlPoint(p0, p3, directionToPlayer, distanceToPlayer, 0.85f);
        
        currentPath = new DrillPath(p0, p1, p2, p3);
        t = 0f;
    }

    /// <summary>
    /// Generates a random path for the boss
    /// Gets current location and generates 3 random points (currently limited by test screensizing)
    /// admittedly just using the above as a template for now until i figure out how
    /// to actually do this
    /// </summary>
    private void CreateChasePathToRandom()
    {
        List<Vector2> pathLocations = new List<Vector2>
        {
            transform.position,
            new Vector2(UnityEngine.Random.Range(-digRange, digRange), UnityEngine.Random.Range(-digRange, digRange)),
            new Vector2(UnityEngine.Random.Range(-digRange, digRange), UnityEngine.Random.Range(-digRange, digRange)),
            new Vector2(UnityEngine.Random.Range(-digRange, digRange), UnityEngine.Random.Range(-digRange, digRange))
        };
        
        currentPath = new DrillPath(pathLocations[0],pathLocations[1],pathLocations[2],pathLocations[3]);
        t = 0f;
    }

    /// <summary>
    /// Generates a random control point between p0 and p3, with restriction that it 
    /// its projection to the line p0 p3 is stricly in between them
    /// Moidfy this (or extend with multiple splines) to make drill path more interesting
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="p3"></param>
    /// <param name="forward"></param>
    /// <param name="distance"></param>
    /// <param name="tAlong"></param>
    /// <returns></returns>
    private Vector2 GenerateControlPoint(Vector2 p0, Vector2 p3, Vector2 forward, float distance, float tAlong)
    {
        Vector2 basePoint = p0 + forward * distance * tAlong;
        
        Vector2 perpendicular = new Vector2(-forward.y, forward.x);
        float randomOffset = UnityEngine.Random.Range(-distance, distance);
        
        Vector2 controlPoint = basePoint + perpendicular * randomOffset;
        
        Vector2 toControl = controlPoint - p0;
        Vector2 toEnd = p3 - p0;
        
        if (Vector2.Dot(toControl, toEnd) < 0)
        {
            controlPoint = basePoint;
        }
        
        return controlPoint;
    }

    /// <summary>
    /// Basically the action that is completed during UG_Chase or UG_Random
    /// When t hits 1, the path is complete.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DigPath()
    {

        // dig movement
        while (t < 1)
        {
            t += Time.deltaTime * speedModifier;
            Vector2 newPos = currentPath.GetPositionForT2D(t);
            transform.position = newPos;
            yield return new WaitForEndOfFrame();
        }
        
        t = 0f;
    }

    /// <summary>
    /// Called when another object enters the trigger collider.
    /// Pushes back the player when the drill guy is digging underneath them.
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            Vector2 dir = (other.transform.position - transform.position).normalized;
            float t = (other.transform.position - transform.position).magnitude / pushRadius;
            float scale = Mathf.Clamp(t, 0.4f, 1f);
            scale = 1f / scale;
            playerRb.AddForce(dir * pushForce * scale, ForceMode2D.Force);
        }
    }

    private void ResetAllAnimatorBools()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isThrowing", false);
        animator.SetBool("isExiting", false);
        animator.SetBool("isEntering", false);
        animator.SetBool("isUG", false);
    }

    public override void TakeDamage(float damage)
    {
        if (!isUnderground) base.TakeDamage(damage);
    }

    public override void SetPhase()
    {

    }
}
