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

    [Header("Bullet Patterns")]
    [SerializeField] BulletPattern machineGun;
    [SerializeField] BulletPattern lazerShot;

    [Header("Attack Constants")]
    [SerializeField] private float machineCooldownConstant;
    private float machineTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        stateTimer = 0;
        machineTimer = 10;
        TransitionToTargeting(); // Set current state to targeting
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        stateTimer -= Time.deltaTime;

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
            TransitionToMachineGun();
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
            //track the player location as you shoot out bullets
            //granny moves as she shoots machine gun
            //if player enters region above or below granny, she stops firing and repositions
            bulletOrigin.transform.right = GameManager.Instance.player.transform.position
                    - bulletOrigin.transform.position;

            if (bulletOrigin.transform.right.x > 0) { sprite.flipX = true; }
            else if (bulletOrigin.right.x < 0) { sprite.flipX = false; }
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
