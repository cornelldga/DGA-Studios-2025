using System;
using System.Collections;
using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

/// <summary>
/// Represents the charater you play in the game. Contains stats, handles inventory, and player inputs
/// </summary>
public class Player : MonoBehaviour, IDamageable

{
    [Header("Stats")]
    public float baseSpeed;
    [HideInInspector] public float speed;
    [SerializeField] float maxHealth;
    private float health;
    [Tooltip("Multiplier for damage taken")]
    public float damageTakenMultiplier;
    [Tooltip("Percent damage dealt back from an enemy projectile")]
    public float whipBaseDamageMultiplier;
    [SerializeField] float changeCooldownTime;
    private bool isAlive;

    [Header("Player Inventory")]
    [SerializeField] private BaseType[] defaultEquippedBases;
    [SerializeField] private MixerType[] defaultEquippedMixers;
    private static BaseType[] equippedBases;      
    private static MixerType[] equippedMixers;   
    PlayerBases playerBases;
    PlayerMixers playerMixers;
    private static int lastBaseIndex = 0; 
    private static int lastMixerIndex = 0; 
    private static bool isInitialized = false;

    [Header("Whip")]
    [SerializeField] Transform whipPivot;
    public Whip whip;
    [SerializeField] float whipCooldownTime;
    [SerializeField] float whipTime;
    [SerializeField] Animator whipAnimator;
    [SerializeField] Animator whipPivotAnimator;
    private bool isMarked;
    private float markTimer;

    [Header("Gun Arm")]
    [SerializeField] Transform armPivot;
    [SerializeField] Animator armAnimator;

    [Header("UI")]
    [SerializeField] Image equippedImage;
    [SerializeField] Image backupImage;
    [SerializeField] Image mixerEquippedImage;
    [SerializeField] Image mixerBackupImage;

    Animator animationControl;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    SpriteRenderer sprite;
    Transform playerTransform;
    float angle;
    Vector2 moveDirection;
    float fireCooldown;
    float changeCooldown;
    float whipCooldown;
    private bool whipping;

    int baseIndex;
    int mixerIndex;
    Base selectedBase;
    Base backupBase;
    Mixer selectedMixer;
    Mixer backupMixer;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        playerMixers = GetComponent<PlayerMixers>();
        playerBases = GetComponent<PlayerBases>();
        animationControl = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerTransform = GetComponent<Transform>();
        // This should be set by the equipped mixer and not by the base stats
        // Introduces issue of checking equipped mixer first, then setting the player stats
        speed = baseSpeed;
        health = maxHealth;
        whip.damageMultiplier = whipBaseDamageMultiplier;
        selectedBase = playerBases.GetBase(equippedBases[0]);
        selectedMixer = playerMixers.GetMixer(equippedMixers[0]);
        SelectBase(0);
        SelectMixer(0);

        equippedImage.sprite = selectedBase.getSprite();
        backupImage.sprite = backupBase.getSprite();
        mixerEquippedImage.sprite = selectedMixer.getSprite();
        mixerBackupImage.sprite = backupMixer.getSprite();

        isAlive = true;
    }
    void Awake()
    {
        if (!isInitialized)
        {
            equippedBases = (BaseType[])defaultEquippedBases.Clone();
            equippedMixers = (MixerType[])defaultEquippedMixers.Clone();
            isInitialized = true;
        }
    }

    void Update()
    {
        if (!isAlive)
        {
            return;
        }
        if (isMarked)
        {
            markTimer -= Time.deltaTime;
            if (markTimer <= 0)
            {
                removeMark();
            }
        }
        fireCooldown -= Time.deltaTime;
        changeCooldown -= Time.deltaTime;
        whipCooldown -= Time.deltaTime;
        PlayerInputs();
    }

    /// <summary>
    /// Removes mark on player.
    /// </summary>
    public void removeMark()
    {
        sprite.color = Color.white;
        isMarked = false;
    }

    /// <summary>
    /// Selects the base using the current baseIndex and swaps out the secondary base
    /// </summary>
    /// <param name="index">The index of the base</param>
    void SelectBase(int index)
    {
        baseIndex = index;
        lastBaseIndex = index;
        selectedBase = playerBases.GetBase(equippedBases[baseIndex]);
        backupBase = playerBases.GetBase(equippedBases[(baseIndex + 1) % equippedBases.Length]);
        equippedImage.sprite = selectedBase.getSprite();
        backupImage.sprite = backupBase.getSprite();
        changeCooldown = changeCooldownTime;
    }

    /// <summary>
    /// Selects the mixer using the current mixerIndex and removes the previous mixer affects
    /// </summary>
    /// <param name="index">The index of the mixer</param>
    void SelectMixer(int index)
    {
        mixerIndex = index;
        selectedMixer.RemoveMixer(this);
        selectedMixer = playerMixers.GetMixer(equippedMixers[mixerIndex]);
        backupMixer = playerMixers.GetMixer(equippedMixers[(mixerIndex + 1) % equippedMixers.Length]);
        selectedMixer.ApplyMixer(this);
        mixerEquippedImage.sprite = selectedMixer.getSprite();
        mixerBackupImage.sprite = backupMixer.getSprite();
        changeCooldown = changeCooldownTime;
    }

    /// <summary>
    /// Read the player's inputs
    /// </summary>
    void PlayerInputs()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = armPivot.transform.position.z;
        armPivot.right = armPivot.transform.position - mousePos;
        
        Vector3 scale = playerTransform.localScale;
        if ((armPivot.transform.position - mousePos).x > 0)
        {
            
            scale.x = 1; // Flip horizontally
            playerTransform.localScale = scale;

        }
        else if ((armPivot.transform.position - mousePos).x < 0)
        {
            armPivot.right = -armPivot.right; //need to flip the pivot so it points the correct way
            scale.x = -1; // Flip horizontally
            playerTransform.localScale = scale;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector2(horizontal, vertical);

        animationControl.SetFloat("Speed", Mathf.Abs(moveDirection.magnitude));



        if (changeCooldown <= 0)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SelectBase((baseIndex + 1) % equippedBases.Length);
            }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                SelectMixer((mixerIndex + 1) % equippedMixers.Length);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1) && baseIndex != 0)
            {
                SelectBase(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && baseIndex != 1)
            {
                SelectBase(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) && mixerIndex != 0)
            {
                SelectMixer(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4) && mixerIndex != 1)
            {
                SelectMixer(1);
            }
        }
        if (!whipping && Input.GetMouseButtonDown(1) && whipCooldown <= 0)
        {
            OnWhip();
            whipCooldown = whipCooldownTime;
        }

        if (Input.GetMouseButton(0) && fireCooldown <= 0)
        {
            armAnimator.Play("Shoot", 0, 0f);
            Fire();
        }
    }
    /// <summary>
    /// Fires the base towards the direction of the mouse with applied Mixer effects
    /// </summary>
    void Fire()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0f;
        Vector3 direction = mouseWorldPos - transform.position;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // TODO: The position should be a transform where the player fires, not the center of the player
        Quaternion fireDirection = Quaternion.Euler(0f, 0f, angle);
        Base baseDrink = Instantiate(selectedBase, transform.position, fireDirection);
        selectedMixer.ApplyMixer(baseDrink);
        fireCooldown = baseDrink.cooldown;
    }

    /// <summary>
    /// Rotates the whip towards the mouse and 
    /// </summary>
    public void OnWhip()
    {
        whipping = true;
        whip.gameObject.GetComponent<EdgeCollider2D>().enabled = true;
        whipPivotAnimator.Play("Whip Rotate", 0, 0f);
        whipAnimator.Play("Whip", 0, 0f);
        StartCoroutine(nameof(WhipTime));
    }

    IEnumerator WhipTime()
    {
        yield return new WaitForSeconds(whipTime);
        whipping = false;
        whip.gameObject.GetComponent<EdgeCollider2D>().enabled = false;
    }
    private void FixedUpdate()
    {
        if (isAlive)
        {
            Move();
        }
    }

    void Move()
    {
        Vector2 direction = new(moveDirection.x, moveDirection.y);
        direction = direction.normalized;
        rb.linearVelocity = direction * speed;
    }

    /// <summary>
    /// Sets the equipped mixer given an appropiate slot index and returns the replaced mixer
    /// it replaced
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <param name="mixerType"></param>
    public MixerType SwapMixerSlot(int slotIndex, MixerType mixer)
    {
        MixerType lastEquippedMixer = equippedMixers[slotIndex];
        equippedMixers[slotIndex] = mixer;
        return lastEquippedMixer;
    }
    /// <summary>
    /// Sets the equipped base given an appropiate slot index and returns the replaced base
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <param name="mixer"></param>
    /// <returns></returns>
    public BaseType SwapBaseSlot(int slotIndex, BaseType baseDrink)
    {
        BaseType lastEquippedBase = equippedBases[slotIndex];
        equippedBases[slotIndex] = baseDrink;
        return lastEquippedBase;
    }
    
    /// <summary>
    /// Updates UI and applied effects when a base or mixer slot changes.
    /// </summary>
    /// <param name="changedSlotIndex"></param>
    /// <param name="isBase"></param>
    public void RefreshUIIfNeeded(int changedSlotIndex, bool isBase)
    {
        if (isBase)
        {
            if (changedSlotIndex == baseIndex)
            {
                selectedBase = playerBases.GetBase(equippedBases[baseIndex]);
                equippedImage.sprite = selectedBase.getSprite();
            }
            backupBase = playerBases.GetBase(equippedBases[(baseIndex + 1) % equippedBases.Length]);
            backupImage.sprite = backupBase.getSprite();
        }
        else
        {
            if (changedSlotIndex == mixerIndex)
            {
                selectedMixer.RemoveMixer(this);
                selectedMixer = playerMixers.GetMixer(equippedMixers[mixerIndex]);
                selectedMixer.ApplyMixer(this);
                mixerEquippedImage.sprite = selectedMixer.getSprite();  
            }
            backupMixer = playerMixers.GetMixer(equippedMixers[(mixerIndex + 1) % equippedMixers.Length]); 
            mixerBackupImage.sprite = backupMixer.getSprite(); 
        }
    }
    public void TakeDamage(float damage)
    {
        health -= damage * damageTakenMultiplier;
        if (health <= 0)
        {
            GameManager.Instance.LoseGame();
        }
    }
    public void ApplyMark(float markDuration)
    {
        isMarked = true;
        sprite.color = new Color(1f, 0.6f, 0.6f);
        markTimer = markDuration;
    }
    public bool IsMarked()
    {
        return isMarked;
    }
    public float GetHealth()
    {
        return health;
    }
    public BaseType[] GetEquippedBases()
    {
        return equippedBases;
    }

    public MixerType[] GetEquippedMixers()
    {
        return equippedMixers;
    } 

    public int GetCurrentBaseIndex()
    {
        return baseIndex;
    }

    public int GetCurrentMixerIndex()
    {
        return mixerIndex;
    }
    
    /// <summary>
    /// Stops the player and all all actions
    /// </summary>
    public void StopPlayer()
    {
        rb.linearVelocity = Vector2.zero;
        animationControl.SetFloat("Speed", 0);
        this.enabled = false;
        
    }
}