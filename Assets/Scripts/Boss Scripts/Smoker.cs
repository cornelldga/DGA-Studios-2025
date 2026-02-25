using System;
using System.Collections.Generic;
using UnityEngine;

public class Smoker : MonoBehaviour
{
    //How fast should the smoker spin. (Can be modified later for different patterns)
    [SerializeField] float spinSpeed = 50;

    private float ogSpeed;
    private SpriteRenderer spriteRenderer;

    [Header("Movement")]
    [Tooltip("Reference to the player transform to move towards")]
    [SerializeField] Transform player;
    [Tooltip("How strongly the smoker is turns towards the player")]
    [SerializeField] float turnSpeed = 2f;
    [Tooltip("How quickly the smoker moves to the player")]
    [SerializeField] float speed = 3f;

    [Tooltip("The player object layer")]
    [SerializeField] LayerMask playerMask;

    private bool collidedWithPlayer;
    private Vector2 currentVelocity;
    private Rigidbody2D rb;

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
        player = GameManager.Instance.player.transform;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.flipX = false;
        collidedWithPlayer = false;


        // Make rigidbody kinematic so player cannot push it
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0;
        }
    }

    /// <summary>
    /// Physics update for following the player
    /// </summary>
    void FixedUpdate()
    {
        if (player != null)
        {
            Vector2 direction = ((Vector2)player.position - rb.position).normalized;
            Vector2 desiredVelocity = direction * speed;
            currentVelocity = Vector2.Lerp(currentVelocity, desiredVelocity, turnSpeed * Time.fixedDeltaTime);
            rb.linearVelocity = currentVelocity;

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
    /// If there is a collision with the player, will activate punch
    /// </summary>
    void OnCollisionEnter(Collision other)
    {
        int otherLayer = other.gameObject.layer;
        if (playerMask.value & (otherLayer << 1) > 0)
        {
            // call Punch coroutine
        }
    }

    // Update is called once per frame
    /// <summary>
    /// On update, the smoke releast point is pivoted to shot in 360 degrees over time. Shootig only happens if enough time has passed.
    /// </summary>
    void Update()
    {


        pivot.transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
        smokeTimer -= Time.deltaTime;
        
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
            Rigidbody2D rb = pellet.GetComponent<Rigidbody2D>();
            pellet.transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f));
            Vector2 direction = new Vector2(releasePoint.position.x - transform.position.x, releasePoint.position.y - transform.position.y).normalized;
            if (rb != null)
            {
                rb.linearVelocity = direction * pelletSpeed * UnityEngine.Random.Range(0.7f, 1.0f) / (i+1);
                rb.angularVelocity = UnityEngine.Random.Range(-30f, 30f);
            }
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

