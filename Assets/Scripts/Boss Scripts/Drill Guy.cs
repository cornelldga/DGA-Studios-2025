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
    private float targetingTime = 1f;
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

    [Header("Dig Settings")]

    [Tooltip("Specifies the max push strength. Increase this to cause more dispalcement from the drill dig path.")]
    [SerializeField] private float pushForce = 30f;
    // the path we take in digging
    private DrillPath currentPath;
    // how far along we are along this path
    private float t;
    // how fast we should complete this dig (DO NOT GO OVER 2 ITS OP!)
    private float speedModifier = 0.8f;
    private float pushRadius = 1f;
    private Collider2D digCollider;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        currentState = State.Walking;
        stateTimer = targetingTime;
        isUnderground = false;
    }

    /// <summary>
    /// Updating of the statemachine.
    /// </summary>
    public override void Update()
    {
        base.Update();
        Debug.Log(currentState);
        stateTimer -= Time.deltaTime;

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
                // Handled by coroutine
                break;
            case State.Entering:
                UpdateEntering();
                break;
            case State.Exiting:
                UpdateExiting();
                break;
        }
    }

    /// <summary>
    /// What the boss does when walking around
    /// Planning for attacking the player is done in Targeting
    /// </summary>
    private void UpdateWalking()
    {
        if (stateTimer <= 0)
        {
            currentState = State.Entering;
        }
    }

    private void UpdateTargeting()
    {
        throw new NotImplementedException();
    }

    private void UpdateUG_Chase()
    {
        if (t >= 1f)
        {
            currentState = State.Exiting;
        }
    }

    private void UpdateUG_Random()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Updates the Drill boss during Entering state.
    /// </summary>
    private void UpdateEntering()
    {
        isUnderground = true;
        Instantiate(enterHolePrefab, transform.position, Quaternion.identity);

        CreateChasePathToPlayer();
        StartCoroutine(DigPath());
        
        currentState = State.Underground_Chase;
    }

    /// <summary>
    /// Updates the Drill boss during Exiting state.
    /// </summary>
    private void UpdateExiting()
    {
        isUnderground = false;
        Instantiate(exitHolePrefab, transform.position, Quaternion.identity);

        currentState = State.Walking;
        stateTimer = 1f;
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
        p3 += directionToPlayer * 1.5f;
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

    /// <summary>
    /// Dynamite throwing.
    /// </summary>
    private void PerformThrow()
    {
        for (int i = 0; i<holes.Count; i++){
            Vector2 target = holes[i].transform.position;
            //Throw dynamite in that direction.
        }
    }

    public override void SetPhase()
    {
        throw new NotImplementedException();
    }
}
