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
  

    private bool isPunching;

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

    CircleCollider2D circleCollider;
    public override void Start()
    {
        base.Start();
        TakeDamage(0);
        circleCollider = GetComponent<CircleCollider2D>();
        sprite = GetComponent<SpriteRenderer>();

        machineTimer = 0;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        UpdateFlip();
    }

    public override void Attack()
    {
        base.Attack();
        int currentAttack = Random.Range(1, 5);
        Debug.Log(currentAttack);
        //switch (currentAttack)
        //{
        //    case 1:
         //StartCoroutine(Punch());
                
        //        break;
        //    case 2:
          StartCoroutine(selectComboAttack());
        //        break;
        //    case 3:
        //        MachineGun();
        //        break;
        //    case 4:
        //        Laser();
        //        break;
        //}
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
        }
        else if (bulletOrigin.right.x < 0) {
            sprite.flipX = false;
        }
    }

    private void Laser()
    {
        // Shouldn't need to do anything
        return;
    }


    private void MachineGun()
    {
        StartCoroutine(machineGun.DoBulletPattern(this));
        //rb.MovePosition(new Vector2(Random.Range(-5, 5), Random.Range(-3, 3)));
    }

    private IEnumerator Punch()
    {
        //Punch
        // punching is true
        // disappear and teleport delay
        // punches
        // punch is false, set attack cooldown to punchCooldown 

        // TODO move granny towards the player while using her punch move that is 
        // maybe make the punch a separate hitbox
        // TODO: Instantiate smoke VFX here
        // Phase 1: Disappear and snap to left of player
        SetAttackState(true);
        Disappear(true);
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(disappearTime);
        Disappear(false);
        Teleport();
        rb.bodyType = RigidbodyType2D.Kinematic;
        animator.SetTrigger("Punch");
                 
   
    }
    /// <summary>
    /// Toggles Granny Disappear stare for punch
    /// </summary>
    /// <param name="disappear"></param>
    private void Disappear(bool disappear)
    {
        sprite.enabled = !disappear;
        circleCollider.enabled = !disappear;
    }
    /// <summary>
    /// Calculates where Granny teleports to based on player position
    /// </summary>
    private void Teleport()
    {
        //randomly pick left or right
        bool leftPunch = Random.value > 0.5f;
        if (leftPunch)
        {
            punchRepositionOffset = -Mathf.Abs(punchRepositionOffset);
        }
        else
        {
            punchRepositionOffset = Mathf.Abs(punchRepositionOffset);
        }


        Vector2 playerPos = GameManager.Instance.player.transform.position;


        if (playerPos.x + punchRepositionOffset < leftBound.transform.position.x)
        {
            // Player too far left, switch to attacking from the right instead
            punchRepositionOffset = Mathf.Abs(punchRepositionOffset);
        }
        else if (playerPos.x + punchRepositionOffset > rightBound.transform.position.x)
        {
            // Player too far right, switch to attacking from the left instead
            punchRepositionOffset = -Mathf.Abs(punchRepositionOffset);
        }

        rb.position = new Vector2(playerPos.x + punchRepositionOffset, playerPos.y);
    }
    /// <summary>
    /// Called when Granny reappears to punch player, enables punch collider and moves forward
    /// </summary>
    public void AnimationPunch()
    {
        punch.SetActive(true);
        Vector2 playerPos = GameManager.Instance.player.transform.position;
        Vector2 direction = new Vector2(playerPos.x - rb.position.x, 0).normalized;
        rb.linearVelocity = direction * punchSpeed;
    }

    public void AnimationPunchComplete()
    {
        punch.SetActive(false);
        attackCooldown = punchingTime;
        rb.bodyType = RigidbodyType2D.Dynamic;
        SetAttackState(false);
    }
    private IEnumerator selectComboAttack()
    {
        // TODO: Cycle through 6 combo attacks from design document
        int currentCombo = Random.Range(1, 7);
        switch (currentCombo)
        {
            case 1:
                yield return StartCoroutine(TrailAttack(flamingBushPrefab, fireBullsCount, 10));
                break;
            case 2:
                yield return StartCoroutine(TrailAttack(smokePrefab, smokeBullsCount, -1));
                break;
             case 3:
                yield return StartCoroutine(TrailAttack(dynamitePrefab, dynamiteBullsCount, -1));
                break;
             case 4:
                yield return StartCoroutine(PointAttack(smokePrefab, flamingBushPrefab, fireSmokeCount, 10));
                break;
             case 5:
                yield return StartCoroutine(PointAttack(dynamitePrefab, flamingBushPrefab, fireDynamiteCount, 1));
                break;
             case 6:
                yield return StartCoroutine(PointAttack(dynamitePrefab, smokePrefab, smokeDynamiteCount, 1));
                break;
        }
        yield return new WaitForSeconds(0.25f);
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

    private IEnumerator ShootLazer()
    {
        bulletOrigin.transform.right = GameManager.Instance.player.transform.position
                    - bulletOrigin.transform.position;
        yield return StartCoroutine(lazerShot.DoBulletPattern(this));
    }

    public override void SetPhase()
    {
        base.SetPhase();
        switch (currentPhase)
        {
            case 1:
                break;
            case 2:
                break;
        }
    }
}
