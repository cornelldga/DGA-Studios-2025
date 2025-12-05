using UnityEngine;
using Unity.Cinemachine;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Splines;

public class DrillGuy : Boss
{
    public enum State
    {
        Walking, Targeting, Underground_Chase, Underground_Random, Throwing, Entering, Exiting
    }
    public State currentState;
    [Header("State Timing")]
    //How much time to get a lock on player.
    [SerializeField] float targetingTime = 1f;
    [SerializeField] float initialThrowTime = 7.2f;
    [SerializeField] float walkingTime = 4f;
    //Time until we should change states.
    private float stateTimer;
    private Rigidbody2D rb;
    private bool isUnderground;
    private CinemachineImpulseSource impulseSource;
    private List<GameObject> holes; //holes

    [Header("Hole Settings")]

    [Tooltip("The drill hole prefab object that is created by Drill Guy's transition from underground to aboveground")]
    [SerializeField] private GameObject exitHolePrefab;

    [Tooltip("The drill hole prefab object that is created by Drill Guy's transition from aboveground to underground")]
    [SerializeField] private GameObject enterHolePrefab;

    // my math ta says epsilon in a funny way so now i like the word, used to not magic number the z ordering
    private float zEpsilon = 0.1f;

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

    // want to look to disable hurtbox if underground. we also need to make Boss.TakeDamage() overridable
    private BoxCollider2D hurtBox;

    // similarly we have a different hitbox for when it is underground
    private CircleCollider2D pushTrigger;

    [Tooltip("Driller Animation Controller")]
    private Animator animator;

    //Time until we should change states.
    [SerializeField] DynamitePattern dynamitePatternPhase1;
    [SerializeField] DynamitePattern dynamitePatternPhase2;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        currentState = State.Walking;
        stateTimer = walkingTime;
        isUnderground = false;
        animator = GetComponent<Animator>();
        holes = new List<GameObject>();
        hurtBox = GetComponent<BoxCollider2D>();
        pushTrigger = GetComponent<CircleCollider2D>();
        pushTrigger.enabled = false;
        // holes.Add(Instantiate(EnterHolePrefab, GameManager.Instance.player.transform.position,  Quaternion.identity)); //for testing throwing at holes
    }

    /// <summary>
    /// Updating of the statemachine.
    /// </summary>
    public override void Update()
    {
        base.Update();
        Debug.Log(currentState);
        Debug.Log(animator.GetCurrentAnimatorStateInfo(0).fullPathHash);
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
        StartCoroutine(dynamitePatternPhase1.ThrowRoutine(bulletOrigin.position, GameManager.Instance.player.transform.position));
    }

     //Throws Dynamite at the holes (phase 2)
    private void ThrowDynamiteAtHoles()
    {
        foreach(GameObject hole in holes)
            StartCoroutine(dynamitePatternPhase2.ThrowRoutine(bulletOrigin.position, hole.transform.position));
        currentState = State.Targeting;
    }

    /// <summary>
    /// Transition to walking.
    /// </summary>
    private void TransitionToWalking()
    {
        animator.SetBool("isWalking", true);
        animator.SetBool("isThrowing", false);
        currentState = State.Walking;
        stateTimer = walkingTime;
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
    }

    /// <summary>
    /// Transition to throwing state
    /// </summary>
    private void TransitionToThrowing()
    {
        animator.SetBool("isThrowing", true);
        animator.SetBool("isExiting", false);
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
        if (attackCooldown <= 0)
        {
            if (currentPhase == 1)
            {
                ThrowDynamiteAtPlayer(); //phase 1
                attackCooldown = dynamitePatternPhase1.cooldown;
            }
            else if (currentPhase == 2) {
                ThrowDynamiteAtHoles(); //phase 2
                currentState = State.Walking;
            }
        }
    }

    private void UpdateTargeting()
    {
        throw new NotImplementedException();
    }

    private void TransitionToUGChase()
    {
        animator.SetBool("isUG", true);
        animator.SetBool("isEntering", false);
        currentState = State.Underground_Chase;
        hurtBox.enabled = false;
        pushTrigger.enabled = true;
        isUnderground = true;
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

    private void UpdateUG_Random()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Transitions to the Entering state
    /// </summary>
    private void TransitionToEntering()
    {
        animator.SetBool("isWalking", false);
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
    private void OnEnteredGround()
    {
        Vector3 spawnPos = transform.position;
        spawnPos.z += zEpsilon;
        holes.Add(Instantiate(enterHolePrefab, spawnPos, Quaternion.identity));
    }

    /// <summary>
    /// Drill entering finish Animation event, transitions to UG
    /// </summary>
    private void OnEnteringFinished()
    {
        if (currentState == State.Entering)
            TransitionToUGChase();
    }

    /// <summary>
    /// Transitions to exiting state
    /// </summary>
    private void TransitionToExiting()
    {
        animator.SetBool("isUG", false);
        animator.SetBool("isExiting", true);
        currentState = State.Exiting;

        isUnderground = false;
        Vector3 spawnPos = transform.position;
        spawnPos.z += zEpsilon;
        Instantiate(enterHolePrefab, spawnPos, Quaternion.identity);
    }

    /// <summary>
    /// Updates the Drill boss during Exiting state.
    /// </summary>
    private void UpdateExiting()
    {
        
    }

    /// <summary>
    /// Animation triggered event to spawn exit hole to match up with animation
    /// </summary>
    private void OnExitedGround()
    {
        Vector3 spawnPos = transform.position;
        spawnPos.z += zEpsilon;
        holes.Add(Instantiate(enterHolePrefab, spawnPos, Quaternion.identity));
    }

    /// <summary>
    /// called when exiting animation finishes, lets state machine know boss can now move on
    /// </summary>
    private void OnExitingFinished()
    {
        hurtBox.enabled = true;
        pushTrigger.enabled = false;
        isUnderground = false;
        if (currentState == State.Exiting)
        {
            TransitionToThrowing();
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
            Debug.Log($"t={t}, position={newPos}");
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
            if (playerRb != null)
            {
                playerRb.AddForce(dir * pushForce * scale, ForceMode2D.Force);
            } else
            {
                Debug.Log("Drill push back trigger can not find PlayerRb");
            }
        }
    }

    public override void TakeDamage(float damage)
    {
        if (!isUnderground) base.TakeDamage(damage);
    }

    public override void SetPhase()
    {
        switch (currentPhase)
        {
            case 1:
                Debug.Log("Phase 1");
                break;
            case 2:
                Debug.Log("Phase 2");
                break;
        }
    }
}
