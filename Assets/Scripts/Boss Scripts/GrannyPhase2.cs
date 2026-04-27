using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class GrannyPhase2 : Boss
{
    public enum State
    {
        Lazer, MachineGun, Punch, ComboAttack
    }
    private State currentState;

    [Header("Movement Settings")]
    //Base speed when charging (regular)
    [SerializeField] float baseSpeed = 5f;

    private Vector2 targetPosition;

    [Header("State Timing")]
    //How much time to get to pull out contracts.
    private float idleTime = 1f;
    //How long we should scavenge for contracts.
    private float scavengeTime = 1f;
    //Length of time to pull out contracts.
    private float outTime = 1f;
    

    private float currentSpeed;
    //Time until we should change states.
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
    //Cooldown on MachineGun bullet waves.
    private float machineTimer = 0f;
    [Header("Combo Attack Constants")]
    [SerializeField] private int fireBullsCount;
    [SerializeField] private int smokeBullsCount;
    [SerializeField] private int dynamiteBullsCount;
    [SerializeField] private int fireSmokeCount;
    [SerializeField] private int fireDynamiteCount;
    [SerializeField] private int smokeDynamiteCount;
    [SerializeField] private float bullsTime;

    [Header("Prefabs")]
    [SerializeField] private Bull bullPrefab;
    [SerializeField] private GameObject flamingBushPrefab;
    [SerializeField] private GameObject smokePrefab;
    [SerializeField] private GameObject dynamitePrefab;

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

    private Coroutine machineGunRoutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        machineTimer = 0;
        stateTimer = 0;
        currentState = State.ComboAttack; // TODO Change
        // TODO remove later. For now, start with Combo Attack
        TransitionToComboAttack();
        // TransitionToLazer(); // Set starting state to lazer
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        stateTimer -= Time.deltaTime;
        UpdateFlip();
        Debug.Log(currentState);
        switch (currentState)
        {
            case State.ComboAttack:
                UpdateComboAttacks();
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

    private void UpdateLazer()
    {
        // Shouldn't need to do anything
    }


    private void UpdateMachineGun()
    {
        //track the player location as you shoot out bullets
        //granny moves as she shoots machine gun
        //if player enters region above or below granny, she stops firing and repositions
        if (machineTimer == 0)
        {
            machineGunRoutine = StartCoroutine(machineGun.DoBulletPattern(this));
        }
        float angle = Vector2.Angle(GameManager.Instance.player.transform.position, this.transform.position);
        print(angle);
        if (angle > 150 || angle < 30)
        {
            StopCoroutine(machineGunRoutine);
            TransitionToComboAttack();
            SetAttackState(false);
        }
        if (machineTimer >= machineCooldownConstant)
        {
            machineTimer = 0;
            TransitionToComboAttack();
        } else
        {
            Debug.Log("Firing");
            machineTimer += Time.deltaTime;
        }

          
    }

    private void UpdatePunch()
    {
        // TODO move granny towards the player while using her punch move that is 
        // maybe make the punch a separate hitbox

        // Phase 1: Disappear and snap to left of player
            if (sprite.enabled)
            {
                // TODO: Instantiate smoke VFX here
                sprite.enabled = false;
                GetComponent<Collider>().enabled = false;
                punch.SetActive(false);   
                //randomly pick left or right
                bool leftPunch = Random.value > 0.5f;
                if (leftPunch)
                {
                    punchRepositionOffset = -Mathf.Abs(punchRepositionOffset);
                } else
                {
                    punchRepositionOffset = Mathf.Abs(punchRepositionOffset); 
                }
                
            }
            
            Vector2 playerPos = GameManager.Instance.player.transform.position;
            
            if (playerPos.x + punchRepositionOffset < leftBound.transform.position.x)
            {
                // Player too far left, switch to attacking from the right instead
                punchRepositionOffset = Mathf.Abs(punchRepositionOffset);
            } else if (playerPos.x + punchRepositionOffset > rightBound.transform.position.x)
            {
                // Player too far right, switch to attacking from the left instead
                punchRepositionOffset = -Mathf.Abs(punchRepositionOffset);
            }

            rb.position = new Vector2(playerPos.x + punchRepositionOffset, playerPos.y);
    

        // Phase 2: Reappear and lunge horizontally
            sprite.enabled = true;
            GetComponent<Collider>().enabled = true;
            punch.SetActive(true);
            Vector2 direction = new Vector2(playerPos.x - rb.position.x, 0).normalized;
            rb.linearVelocity = direction* punchSpeed;
        // Phase 3: Done
            punch.SetActive(false);
            TransitionToMachineGun();
    }

    private void TransitionToPunch()
    {
        currentState = State.Punch;
        stateTimer = disappearTime + punchingTime;
        //play the animation for granny disappearing
    }

    private void UpdateComboAttacks()
    {
        // Shouldn't need to do anything here
    }

    private void TransitionToMachineGun()
    {
        currentState = State.MachineGun;
        machineTimer = 0;
    }

    private void TransitionToComboAttack()
    {
        currentState = State.ComboAttack;
        StartCoroutine(selectComboAttack());
    }

    private IEnumerator selectComboAttack()
    {
        // TODO: Cycle through 6 combo attacks from design document
        yield return StartCoroutine(TrailAttack(flamingBushPrefab, fireBullsCount, 10));
        yield return new WaitForSeconds(1.00f);
        yield return StartCoroutine(TrailAttack(smokePrefab, smokeBullsCount, -1));
        yield return new WaitForSeconds(1.00f);
        yield return StartCoroutine(TrailAttack(dynamitePrefab, dynamiteBullsCount, -1));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(PointAttack(smokePrefab, flamingBushPrefab, fireSmokeCount, 10));
        yield return new WaitForSeconds(1.00f);
        yield return StartCoroutine(PointAttack(dynamitePrefab, flamingBushPrefab, fireDynamiteCount, 1));
        yield return new WaitForSeconds(1.00f);
        yield return StartCoroutine(PointAttack(dynamitePrefab, smokePrefab, smokeDynamiteCount, 1));

        yield return new WaitForSeconds(0.25f);
        Debug.Log("Transitioning from combo to punch");
        TransitionToPunch();
    }

    private IEnumerator TrailAttack(GameObject prefab, int bullsCount, float trailLifeTime)
    {
        for (int i = 0; i < bullsCount; i++)
        {
            Bull bull = Instantiate(bullPrefab, this.transform.position, Quaternion.identity);
            bull.ChargeSpecificDirection(Random.onUnitSphere);
            bull.setSummoned();
            
            Trail trail = bull.AddComponent<Trail>();
            trail.SetTrailPrefab(prefab);
            trail.SetTrailLifetime(trailLifeTime);

            Destroy(bull, bullsTime);
        }

        yield return new WaitForSeconds(bullsTime);
    }

    private IEnumerator PointAttack(GameObject prefab, GameObject SecondaryPrefab, int ProjectileCount, float ProjectileLifeTime)
    {
        for (int i = 0; i < ProjectileCount; i++)
        {
            GameObject Primary = Instantiate(prefab, new Vector3(Random.Range(-5,5),Random.Range(-3,3),-1), Quaternion.identity);

            Point Secondary = Primary.AddComponent<Point>();
            Secondary.SetSecondaryPrefab(SecondaryPrefab);
            
            //Secondary.DropSecondaryProjectile();

            Destroy(Primary, ProjectileLifeTime);
            Secondary.DropSecondaryProjectile();
        }
        yield return new WaitForSeconds(3f); 
    }
    
    private void TransitionToLazer()
    {
        currentState = State.Lazer;
        //showcase the lazer tuning on and honing in on the player
        //shoot the lazer for the 1 shot hit
        StartCoroutine(ShootLazer());
    }

    private IEnumerator ShootLazer()
    {
        bulletOrigin.transform.right = GameManager.Instance.player.transform.position
                    - bulletOrigin.transform.position;
        yield return StartCoroutine(lazerShot.DoBulletPattern(this));
        TransitionToPunch();
    }

    public override void SetPhase()
    {
        switch (currentPhase)
        {
            case 1:
                break;
            case 2:
                currentState = State.Lazer;
                break;
        }
    }
}
