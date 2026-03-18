using UnityEngine;

public class GrannyPhase2 : Boss
{
    /// <summary>
    /// Granny will start Phase 2 with Lazer, then cycle between
    /// (1) Punch -> (4s) MachineGun -> (1) ComboAttack -> Loop
    /// Until desperation, where she does another lazer then goes back to loop
    /// </summary>
    public enum State
    {
        Targeting, Lazer, MachineGun, Punch, ComboAttack
    }
    private State currentState;

    [Header("Movement Settings")]
    //Base speed when charging (regular)
    [SerializeField] float baseSpeed = 5f;

    private Vector2 targetPosition;

    [Header("State Timing")]
    //How much time to get to pull out contracts.
    private float stateTimer;
    private float firingCooldown;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sprite;
    private Collider2D collider;

    [Header("Bullet Patterns")]
    [SerializeField] BulletPattern machineGun;
    [SerializeField] BulletPattern lazerShot;

    [Header("Attack Constants")]
    [SerializeField] private float machineCooldownConstant;
    private float machineTimer;

    [Header("Punch Move")]
    //how long granny is punching for
    [SerializeField] private float punchingTime;
    //how long granny disppears for
    [SerializeField] private float disappearTime;
    [SerializeField] private float punchRepositionOffset;
    [SerializeField] private float punchSpeed;
    [SerializeField] GameObject punch;
    [SerializeField] Transform punchPivot;

    //left and right bound essentially tell us how far left/right is
    //"too far". This is used to prevent granny from trying to teleport 
    // left if the player is hugging the left wall for example
    [SerializeField] Transform leftBound;
    [SerializeField] Transform rightBound;

    //if false, we punching from the right
    //if true, we punching from the left
    bool leftPunch;

    //vector to remember how we were moving after determining punch move direction
    private Vector2 punchMove;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        collider = GetComponent<CircleCollider2D>();
        machineTimer = 0;
        machineTimer = 10;
        TransitionToPunch();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        stateTimer -= Time.deltaTime;
        UpdateFlip();
        switch (currentState)
        {
            case State.Targeting:
                UpdateTargeting();
                break;
            case State.Lazer:
                UpdateLazer();
                break;
            case State.MachineGun:
                UpdateMachineGun();
                break;
            case State.Punch:
                UpdatePunch();
                break;
        }
    }

    /// <summary>
    /// Checks if the player sprite needs to update and updates the bullet origin
    /// </summary>
    private void UpdateFlip(){
        //track the player location as you shoot out bullets
        //granny moves as she shoots machine gun
        //if player enters region above or below granny, she stops firing and repositions
        bulletOrigin.transform.right = GameManager.Instance.player.transform.position
                - bulletOrigin.transform.position;

        if (bulletOrigin.transform.right.x > 0) { 
            sprite.flipX = true;
            punchPivot.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (bulletOrigin.right.x < 0) { 
            sprite.flipX = false; 
            punchPivot.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
    }

    /// <summary>
    /// Handles logic for targeting mode.
    /// </summary>
    private void UpdateTargeting()
    {
        // Track the player's position
        if (GameManager.Instance != null && GameManager.Instance.player != null)
        {
            targetPosition = GameManager.Instance.player.transform.position;
        }

        // When targeting time is up, decide what to do
        if (stateTimer <= 0)
        {
            TransitionToPunch();
        }
    }

    private void UpdateLazer()
    {
        //showcase the lazer tuning on and honing in on the player
        //shoot the lazer for the 1 shot hit
        bulletOrigin.transform.right = GameManager.Instance.player.transform.position
                    - bulletOrigin.transform.position;
        StartCoroutine(lazerShot.DoBulletPattern(this));
    }

    private void UpdateMachineGun()
    {
        if (machineTimer == 0)
        {
            
            StartCoroutine(machineGun.DoBulletPattern(this));
            float angle = Vector2.Angle(GameManager.Instance.player.transform.position, this.transform.position);
            print(angle);
            if (angle < 20 || angle > 170)
            {
                StopCoroutine(machineGun.DoBulletPattern(this));
                TransitionToTargeting();
                return;
            }
            machineTimer += Time.deltaTime;

        }
        else if (machineTimer >= machineCooldownConstant)
        {
            machineTimer = 0;
        }
        else
        {
            machineTimer += Time.deltaTime;
        }
    }

    private void UpdatePunch()
    {
        // TODO move granny towards the player while using her punch move that is 
        // maybe make the punch a separate hitbox

        // Phase 1: Disappear and snap to left of player
        if (stateTimer > punchingTime)
        {
            if (sprite.enabled)
            {
                // TODO: Instantiate smoke VFX here
                sprite.enabled = false;
                collider.enabled = false;
                punch.SetActive(false);   
                //randomly pick left or right
                bool leftPunch = Random.value > 0.5f;
                if (leftPunch)
                {
                    punchRepositionOffset = -punchRepositionOffset;
                } else
                {
                    
                }
            }
            
            //continually check if the player location goes over the preferred bounds in case we need to change direction
            if (leftPunch)
            {
                //check if we should change because the player is too left

            } else
            {
                //check if we should change because the player is too right

            }

            // Snap to right/left of player, aligned horizontally
            Vector2 playerPos = GameManager.Instance.player.transform.position;
            if (leftPunch)
            {
                 rb.position = new Vector2(punchRepositionOffset - playerPos.x , playerPos.y);
            } else
            {
                rb.position = new Vector2(playerPos.x - punchRepositionOffset, playerPos.y);
            }
            
        }
        // Phase 2: Reappear and lunge horizontally
        else if (stateTimer > 0)
        {
            sprite.enabled = true;
            collider.enabled = true;
            punch.SetActive(true);

            Vector2 playerPos = GameManager.Instance.player.transform.position;
            Vector2 direction = new Vector2(playerPos.x - rb.position.x, 0).normalized;
            rb.position += direction * punchSpeed * Time.deltaTime;
        }
        // Phase 3: Done
        else
        {
            punch.SetActive(false);
            TransitionToPunch();    
        }
    }

    private void TransitionToPunch()
    {
        currentState = State.Punch;
        stateTimer = disappearTime + punchingTime;
        //play the animation for granny disappearing
    }

    private void UpdateComboAttacks()
    {
        // TODO: Cycle through 6 combo attacks from design document
    }

    private void TransitionToTargeting()
    {
        currentState = State.Targeting;
    }

    private void TransitionToMachineGun()
    {
        currentState = State.MachineGun;
    }

    private void TransitionToLazer()
    {
        currentState = State.Lazer;
    }

    public override void SetPhase()
    {
        switch (currentPhase)
        {
            case 1:
                break;
            case 2:
                TransitionToLazer();
                break;
        }
    }
}
