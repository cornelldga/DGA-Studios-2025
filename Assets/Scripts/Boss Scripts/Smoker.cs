using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Smoker : MonoBehaviour
{
    //How fast should the smoker spin. (Can be modified later for different patterns)
    [SerializeField] float spinSpeed = 50;

    private float ogSpeed;
    private SpriteRenderer spriteRenderer;

    [Header("Movement")]
    [Tooltip("Reference to the player transform to move towards")]
    [HideInInspector] Transform player;
    [Tooltip("How strongly the smoker is turns towards the player")]
    [SerializeField] float turnSpeed = 2f;
    [Tooltip("How quickly the smoker moves to the player")]
    [SerializeField] float speed = 3f;

    [Tooltip("The player object layer")]
    [SerializeField] LayerMask playerMask;

    // [Tooltip("The screen tinting for the smoke")]
    [Header("Smoke Tinting")]
    [SerializeField] GameObject smokeTint;
    private bool collidedWithPlayer;
    private Vector2 currentVelocity;
    private Rigidbody2D rb;
    private Animator animator;

    //How fast should smoke be shot out of smoker pipe.
    [SerializeField] float pelletSpeed = 3;
    [Tooltip("Fire rate of smoke")]
    [SerializeField] float smokeRate;
    //A counter on how long it has been since the last smoke was shot.
    float smokeTimer = 0f;
    [SerializeField] GameObject smokePelletPrefab;
    //Where the smoke should be released from.
    [SerializeField] Transform releasePoint;
    //A transform centered on the smoker to allow for 360 smoking.
    [SerializeField] Transform pivot;
    [SerializeField] TheMagician magician;

    [Header("Punch Settings")]
    [Tooltip("How long must pass before this can perform another punch")]
    private bool playerInRange = false;
    [SerializeField] float cooldown;
    [SerializeField] float knockTime;
    [SerializeField] float punchDist;
    [SerializeField] float punchMagnitude;

    private bool isPunching;



    [Header("Stages")]
    [SerializeField] Transform backStage;
    [SerializeField] Transform cardStage;
    [SerializeField] Transform doveStage;
    [SerializeField] Transform knifeStage;


    // Whether the stage has been hidden yet
    private bool hidStage;
    // If its the first time the magician is off stage
    private bool first;

    void Start()
    {
        first = true;
        hidStage = true;
        ogSpeed = spinSpeed;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        spriteRenderer.flipX = false;
        collidedWithPlayer = false;
        


        // Make rigidbody kinematic so player cannot push it
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0;
        }
    }

    /// <summary>
    /// Physics update for following the player
    /// </summary>
    void FixedUpdate()
    {
        if (player == null && GameManager.Instance?.player != null)
            player = GameManager.Instance.player.transform;

        if (rb != null && player != null && !isPunching)
        {
            Vector2 direction = ((Vector2)player.position - rb.position).normalized;
            Vector2 desiredVelocity = direction * speed;
            currentVelocity = Vector2.Lerp(currentVelocity, desiredVelocity, turnSpeed * Time.fixedDeltaTime);
            rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);

            // Flip sprite based on player position
            if (spriteRenderer != null)
            {
                if (player.position.x < transform.position.x)
                {
                    spriteRenderer.flipX = false; // Player is to the left
                }
                else
                {
                    spriteRenderer.flipX = true; // Player is to the right
                }
            }
        }
    }

    /// <summary>
    /// waits for cooldown seconds then attempts punch
    /// </summary>
    IEnumerator PunchRoutine(GameObject target)
    {
        isPunching = true;
        rb.linearVelocity = Vector2.zero;
        currentVelocity = Vector2.zero;
        animator.SetBool("Walking", false);

        Player playerScript = target.GetComponent<Player>();
        if (playerScript == null) { isPunching = false; yield break; }

        yield return new WaitForSeconds(cooldown);

        if (target != null && playerInRange)
        {
            Vector2 dist2D = (Vector2)(target.transform.position - transform.position);
            if (dist2D.magnitude < punchDist)
            {
                Vector2 direction = dist2D.normalized;
                playerScript.knockedBack = true;
                Rigidbody2D playerBody = target.GetComponent<Rigidbody2D>();
                float pastDamping = playerBody.linearDamping;
                playerBody.linearDamping = 3f;
                playerBody.AddForce(direction * punchMagnitude, ForceMode2D.Impulse);
                yield return new WaitForSeconds(knockTime);
                playerBody.linearDamping = pastDamping;
                playerScript.knockedBack = false;
            }
        }

        isPunching = false;
    }

    /// <summary>
    /// If there is a collision with the player, will activate punch
    /// </summary>
    void OnCollisionEnter2D(Collision2D other)
    {
        if (!isPunching && (playerMask.value & (1 << other.gameObject.layer)) > 0)
        {
            playerInRange = true;
            StartCoroutine(PunchRoutine(other.gameObject));
        }
    }

    /// <summary>
    /// While colliding with the player, starts the punch. Can punch again.
    /// </summary>
    void OnCollisionStay2D(Collision2D other)
    {
        if (!isPunching && (playerMask.value & (1 << other.gameObject.layer)) > 0)
        {
            playerInRange = true;
            StartCoroutine(PunchRoutine(other.gameObject));
        }
    }

    /// <summary>
    /// Cant punch until player is in the range again.
    /// </summary>
    void OnCollisionExit2D(Collision2D other)
    {
        if ((playerMask.value & (1 << other.gameObject.layer)) > 0)
            playerInRange = false;
    }

    // Update is called once per frame
    /// <summary>
    /// On update, the smoke releast point is pivoted to shot in 360 degrees over time. Shootig only happens if enough time has passed.
    /// </summary>
    void Update()
    {


        pivot.transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
        smokeTimer -= Time.deltaTime;
        animator.SetBool("Walking", currentVelocity.magnitude > 0.1f);
        
        if (magician.currentStage == Stage.Backstage )
        {
            if(smokeTimer <= 0 & !first)
            {
                if (!hidStage)
                {
                    int i =UnityEngine.Random.Range(0,4);
                    switch (i) {
                    case 0:
                        SetLeft();
                        
                        break;
                    case 1: 
                        SetRight(); 
                        
                        break;
                    case 2:
                        SetUp();
                        
                        break;
                    case 3:
                        SetAround();

                        break;
                    }
                
                    ObscureStage();
                }

                ShootSmoke();
                smokeTimer = smokeRate;
                
            }
        }
        else { 
            hidStage = false;
            first = false;
        }

    }
    /// <summary>
    /// Instantiates three pellets of smoke to be shot. It will rotate, and fire in the given direction. 
    /// </summary>
    void ShootSmoke()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject pellet = Instantiate(smokePelletPrefab, releasePoint.transform.position, Quaternion.identity);
            pellet.setTint(smokeTint);
            Rigidbody2D rb = pellet.GetComponent<Rigidbody2D>();
            pellet.transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f));
            Vector2 direction = new Vector2(releasePoint.position.x - transform.position.x, releasePoint.position.y - transform.position.y).normalized;
            if (rb != null)
            {
                rb.linearVelocity = direction * pelletSpeed * UnityEngine.Random.Range(0.7f, 1.0f) / (i+1);
                rb.angularVelocity = UnityEngine.Random.Range(-30f, 30f);
            }
            SmokePool.Instance.AddToPool(pellet);
        }
    }

    /// <summary>
    /// Sets the pivot to shoot at the left side of the arena
    /// </summary>
    void SetLeft()
    {
        pivot.transform.rotation = Quaternion.Euler(0,0,45);
        spinSpeed = ogSpeed;
    }

    /// <summary>
    /// Sets the pivot to shoot at the right side of the arena
    /// </summary>
    void SetRight()
    {
        pivot.transform.rotation = Quaternion.Euler(0, 0, -45);
        spinSpeed = -ogSpeed;
        
    }

    /// <summary>
    /// Sets the pivot to shoot at the upper part of the arena
    /// </summary>
    void SetUp()
    {
        pivot.transform.rotation = Quaternion.Euler(0, 0, -45);
        spinSpeed = ogSpeed;
    }

    /// <summary>
    /// Sets the pivot to shoot all around the arena
    /// </summary>
    void SetAround()
    {
        pivot.transform.rotation = Quaternion.Euler(0, 0, 0);
        spinSpeed = ogSpeed * 4;
    }



    /// <summary>
    /// Hides 2 of the three stages with a single smoke pellet
    /// </summary>
    void ObscureStage()
    {
        List<Stage> stages = new List<Stage>();
        stages.Add(Stage.Card);
        stages.Add(Stage.Knife);
        stages.Add(Stage.Dove);
        for (int i = 0; i < 2; i++)
        {
            Stage hideStage = stages[UnityEngine.Random.Range(0, stages.Count)];
            stages.Remove(hideStage);

            switch (hideStage)
            {
                case Stage.Card:
                    Instantiate(smokePelletPrefab, cardStage.position, Quaternion.identity); 
                    break;

                case Stage.Dove:
                    Instantiate(smokePelletPrefab, doveStage.position, Quaternion.identity);
                    break;
                case Stage.Knife:
                    Instantiate(smokePelletPrefab, knifeStage.position, Quaternion.identity);
                    break;
            }
        }
        hidStage = true;
    }


}

