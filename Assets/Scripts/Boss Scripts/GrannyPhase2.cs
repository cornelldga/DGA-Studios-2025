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
    [SerializeField] private float flamingBullsCount;
    [SerializeField] private float flamingBullsTime;

    [Header("Prefabs")]
    [SerializeField] private GameObject bullPrefab;
    [SerializeField] private GameObject bushPrefab;

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
        Debug.Log(currentState);
        switch (currentState)
        {
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

    private void UpdateLazer()
    {
        // Shouldn't need to do anything
    }


    private void UpdateMachineGun()
    {
            //track the player location as you shoot out bullets
            //granny moves as she shoots machine gun
            //if player enters region above or below granny, she stops firing and repositions
            bulletOrigin.transform.right = GameManager.Instance.player.transform.position
                    - bulletOrigin.transform.position;

        if (bulletOrigin.transform.right.x > 0) { sprite.flipX = true; }
        else if (bulletOrigin.right.x < 0) { sprite.flipX = false; }
        if (machineTimer == 0)
        {
            StartCoroutine(machineGun.DoBulletPattern(this));
        }
        float angle = Vector2.Angle(GameManager.Instance.player.transform.position, this.transform.position);
        print(angle);
        if (angle < 50 || angle > 120)
        {
            StopCoroutine(machineGun.DoBulletPattern(this));
            TransitionToComboAttack();
            SetAttackState(false);           
        }
        if (machineTimer >= machineCooldownConstant)
        {
            machineTimer = 0;
        } else
        {
            machineTimer += Time.deltaTime;
        }

          
    }

    private void UpdatePunch()
    {
        // TODO move granny towards the player while using her punch move that is 
        // maybe make the punch a separate hitbox
        TransitionToMachineGun();
    }

    private void UpdateComboAttacks()
    {
        // Shouldn't need to do anything here
    }

    private void TransitionToMachineGun()
    {
        currentState = State.MachineGun;
    }

    private void TransitionToComboAttack()
    {
        currentState = State.ComboAttack;
        StartCoroutine(selectComboAttack());
    }

    private IEnumerator selectComboAttack()
    {
        // TODO: Cycle through 6 combo attacks from design document
        yield return StartCoroutine(FlamingBullsAttack());

        yield return new WaitForSeconds(0.25f);
        Debug.Log("Transitioning from combo to punch");
        // TransitionToPunch(); TODO uncomment
    }

    private IEnumerator FlamingBullsAttack()
    {
        List<GameObject> bulls = new List<GameObject>();
        for (int i = 0; i < flamingBullsCount; i++)
        {
            bulls.Add(Instantiate(bullPrefab, this.transform.position, Quaternion.identity));

            Pig bullScript = bulls[i].GetComponent<Pig>();
            bullScript.ChargeSpecificDirection(Random.onUnitCircle);
            bullScript.setSummoned();
            
            Trail bushTrail = bulls[i].AddComponent<Trail>();
            bushTrail.SetTrailPrefab(bushPrefab);
            
        }
        yield return new WaitForSeconds(flamingBullsTime);
        for (int i = bulls.Count - 1; i >= 0; i--)
        {
            Destroy(bulls[i]);
            bulls.RemoveAt(i);
        }
    }

    private void PointlAttacks()
    {
        
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

    private void TransitionToPunch()
    {
        currentState = State.Punch;
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
