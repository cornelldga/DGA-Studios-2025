using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Smoker : MonoBehaviour, IProjectileInteractable
{
    //How fast should the smoker spin. (Can be modified later for different patterns)
    [SerializeField] float spinSpeed = 50;

    [SerializeField] float whipLaunchSpeed;
    [SerializeField] float smokerWhipStunTime;

    private float ogSpeed;
    private SpriteRenderer spriteRenderer;

    [Header("Movement")]
    [SerializeField] float maxSpeed;
    [Tooltip("How quickly the smoker moves to the player")]
    [SerializeField] float speed = 3f;
    [Tooltip("Multiplied with the projectile damage to push the smoker back")]
    [SerializeField] float knockbackForce;

    [Tooltip("The player object layer")]
    [SerializeField] LayerMask playerMask;

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
    [SerializeField] float punchCooldown;
    float punchCooldownTime;
    [SerializeField] float punchDamping;
    [SerializeField] float knockTime;
    [SerializeField] float punchMagnitude;
    [SerializeField] float punchRange;

    private bool isPunching;
    [Tooltip("The distance between the Smoker and Player")]
    Vector2 distance;



    [Header("Stages")]
    [SerializeField] Transform backStage;
    [SerializeField] Transform cardStage;
    [SerializeField] Transform doveStage;
    [SerializeField] Transform knifeStage;


    // Whether the stage has been hidden yet
    private bool hidStage;
    // If its the first time the magician is off stage
    private bool first;

    private float smokeSoundCooldown = 5.0f;
    private float smokeSoundTimer = 0f;

    BoxCollider2D smokerCollider;

    float pastDamping;

    void Start()
    {
        first = true;
        hidStage = true;
        ogSpeed = spinSpeed;
        rb = GetComponent<Rigidbody2D>();
        smokerCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        spriteRenderer.flipX = false;
    }

    /// <summary>
    /// Physics update for following the player
    /// </summary>
    void FixedUpdate()
    {

        if (rb != null && GameManager.Instance?.player != null && !isPunching)
        {
            Vector2 direction = ((Vector2)GameManager.Instance.player.transform.position - rb.position);
            rb.AddForce(direction * speed, ForceMode2D.Force);


            if (rb.linearVelocity.magnitude > maxSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;

            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = GameManager.Instance.player.transform.position.x > transform.position.x;
            }
        }
    }

    /// <summary>
    /// waits for cooldown seconds then attempts punch
    /// </summary>
    void Punch()
    {
        isPunching = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        currentVelocity = Vector2.zero;
        animator.SetBool("Walking", false);
        animator.SetTrigger("Punch");
    }
    /// <summary>
    /// Called when smoker swings a punch
    /// </summary>
    public void AnimationSwingPunch()
    {
        Player playerScript = GameManager.Instance.player;
        Vector2 dist2D = playerScript.transform.position - transform.position;

        if (dist2D.magnitude <= punchRange)
        {
            GameManager.Instance.FreezePlayer(true);
            Vector2 direction = dist2D.normalized;
            Rigidbody2D playerBody = playerScript.GetComponent<Rigidbody2D>();
            pastDamping = playerBody.linearDamping;
            playerBody.linearDamping = punchDamping;
            playerBody.AddForce(direction * punchMagnitude, ForceMode2D.Impulse);
        }       
    }
    /// <summary>
    /// Called when the smoker is done swinging his punch
    /// </summary>
    public void AnimationSwingPunchComplete()
    {
        GameManager.Instance.FreezePlayer(false);
        GameManager.Instance.player.GetComponent<Rigidbody2D>().linearDamping = pastDamping;
        isPunching = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        punchCooldownTime = punchCooldown;
    }

    // Update is called once per frame
    /// <summary>
    /// On update, the smoke releast point is pivoted to shot in 360 degrees over time. Shootig only happens if enough time has passed.
    /// </summary>
    void Update()
    {
        pivot.transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
        smokeTimer -= Time.deltaTime;
        smokeSoundTimer -= Time.deltaTime;
        punchCooldownTime -= Time.deltaTime;
        distance = GameManager.Instance.player.transform.position - transform.position;
        if (!isPunching && distance.magnitude <= punchRange && punchCooldownTime <= 0)
        {
            Punch();
        }
        animator.SetBool("Walking", rb.linearVelocity.magnitude > 0.1f);
        
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

                if (smokeSoundTimer <= 0f) {
                    AudioManager.Instance.PlaySFX(SFXKey.SMOKER_AMBIANCE, false);
                    smokeSoundTimer = smokeSoundCooldown;
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
    /// <summary>
    /// Applies a backwards force to the smoker, and damage affecting the strength of
    /// the knockback
    /// </summary>
    public bool ProjectileInteraction(Projectile projectile)
    {
        Vector2 forceDir = (transform.position - projectile.transform.position).normalized;
        rb.AddForce(forceDir * knockbackForce * projectile.damage, ForceMode2D.Impulse);
        return true;
    }
}

