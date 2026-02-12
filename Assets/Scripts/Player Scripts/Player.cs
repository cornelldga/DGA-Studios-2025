
using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    [Tooltip("Seconds player is invulnerable after taking damage")]
    [SerializeField] float invulnerabilityTime;
    bool invulnerable;

    [Header("Player Inventory")]
    BaseType[] equippedBases;
    MixerType[] equippedMixers;
    PlayerBases playerBases;
    PlayerMixers playerMixers;

    [Header("Whip")]
    [SerializeField] Transform whipPivot;
    public Whip whip;
    [SerializeField] float whipCooldownTime;
    [SerializeField] Animator whipAnimator;

    [Header("Gun Arm")]
    [SerializeField] Transform armPivot;
    [SerializeField] Animator armAnimator;
    [SerializeField] Transform bulletOrigin;
    Vector3 bulletRight;

    [Header("UI")]
    [SerializeField] Image equippedImage;
    [SerializeField] Image backupImage;
    [SerializeField] Image mixerEquippedImage;
    [SerializeField] Image mixerBackupImage;
    [SerializeField] Image healthImage;
    [SerializeField] GameObject whipUI;
    [SerializeField] Image whipFillImage;
    [SerializeField] TMP_Text whipCooldownText;

    // Sprite fields temporary. These should be removed and change the health animator
    // [SerializeField] Animator healthAnimator;
    [SerializeField] Sprite healthySprite;
    [SerializeField] Sprite midSprite;
    [SerializeField] Sprite lowHealthSprite;
    [SerializeField] float midHealthThreshold;
    [SerializeField] float criticalThreshold;

    [Header("Mixer Effect")]
    [SerializeField] ParticleSystem mixerEffect;

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
        bulletRight = bulletOrigin.right;
        whip.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        whipUI.SetActive(false);

        speed = baseSpeed;
        health = maxHealth;
        whip.damageMultiplier = whipBaseDamageMultiplier;
        GameManager.Instance.GetLoadout(ref equippedBases, ref equippedMixers);
        selectedBase = playerBases.GetBase(equippedBases[0]);
        selectedMixer = playerMixers.GetMixer(equippedMixers[0]);
        SelectBase(0);
        SelectMixer(0);

        equippedImage.sprite = selectedBase.getSprite();
        backupImage.sprite = backupBase.getSprite();
        mixerEquippedImage.sprite = selectedMixer.getSprite();
        mixerBackupImage.sprite = backupMixer.getSprite();

        isAlive = true;
        GameManager.Instance.player = this;
    }

    void Update()
    {
        if (!isAlive)
        {
            return;
        }
        fireCooldown -= Time.deltaTime;
        changeCooldown -= Time.deltaTime;
        whipCooldown -= Time.deltaTime;
        if (whipCooldown > 0)
        {
            UpdateWhipUI();
        }
        PlayerInputs();
    }


    /// <summary>
    /// Selects the base using the current baseIndex and swaps out the secondary base
    /// </summary>
    /// <param name="index">The index of the base</param>
    void SelectBase(int index)
    {
        baseIndex = index;
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
        mousePos.z = 0f;
        Vector3 scale = transform.localScale;
        scale.x = mousePos.x < transform.position.x ? -1 : 1;
        transform.localScale = scale;

        Vector3 aimDir = mousePos - armPivot.position;
        aimDir.z = 0;
        aimDir.Normalize();

        // Set rotation in world space - this ignores parent flip
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        armPivot.rotation = Quaternion.Euler(0, 0, angle);

        // Counter the parent's scale flip so the arm sprite doesn't get mirrored
        Vector3 armScale = armPivot.localScale;
        armScale.x = scale.x; // This cancels out the parent's flip
        armPivot.localScale = armScale;


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
        //Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        //mouseWorldPos.z = 0f;
        //Vector3 direction = mouseWorldPos - bulletOrigin.position;
        //angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //// TODO: The position should be a transform where the player fires, not the center of the player
        //Quaternion fireDirection = Quaternion.Euler(0f, 0f, angle);
        Base baseDrink = Instantiate(selectedBase, bulletOrigin.position, bulletOrigin.rotation);
        selectedMixer.ApplyMixer(baseDrink);
        fireCooldown = baseDrink.cooldown;
    }

    /// <summary>
    /// Rotates the whip towards the mouse and 
    /// </summary>
    public void OnWhip()
    {
        whipping = true;
        whip.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        Vector3 mouse = Mouse.current.position.ReadValue();
        mouse.z = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 world = Camera.main.ScreenToWorldPoint(mouse);

        Vector3 dir = transform.localScale.x < 0 ? - (world - whipPivot.position) : world - whipPivot.position;
        whipPivot.right = dir;
        whipAnimator.Play("Whip", 0, 0f);
        StartCoroutine(nameof(ToggleWhipUI));
    }
    /// <summary>
    /// Function called by Animator to end the whip
    /// </summary>
    public void AnimationEndWhip()
    {
        whipping = false;
        whip.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        whipPivot.transform.localEulerAngles = Vector3.zero;
    }
    /// <summary>
    /// Toggles the whip UI on when the player whips and off when the cooldown is complete
    /// </summary>
    IEnumerator ToggleWhipUI()
    {
        whipUI.SetActive(true);
        yield return new WaitForSeconds(whipCooldownTime);
        whipUI.SetActive(false);
    }
    /// <summary>
    /// Fill the whip cooldown indicator image as the whip is on cooldown
    /// </summary>
    void UpdateWhipUI()
    {
        whipFillImage.fillAmount = 1 - whipCooldown / whipCooldownTime;
        whipCooldownText.text = whipCooldown.ToString("F1");
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
        SelectMixer(slotIndex);
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
        SelectBase(slotIndex);
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
        if (!invulnerable && damage > 0)
        {
            StartCoroutine(Invulnerability());
            health -= damage * damageTakenMultiplier;
            float healthRatio = health / maxHealth;
            if (healthRatio <= midHealthThreshold)
            {
                healthImage.sprite = midSprite;
                // Should set this boolean animation to true
            }
            if (healthRatio <= criticalThreshold)
            {
                healthImage.sprite = lowHealthSprite;
                // Should set this boolean animation to true
            }
            if (health <= 0)
            {
                GameManager.Instance.LoseGame();
            }
        }
    }
    /// <summary>
    /// Makes the player invulnerable from damage for a short duration
    /// </summary>
    IEnumerator Invulnerability()
    {
        invulnerable = true;
        sprite.color = Color.red;
        yield return new WaitForSeconds(invulnerabilityTime);
        sprite.color = Color.white;
        invulnerable = false;
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

    public void ChangeMixerEffect(Color mixerColor)
    {
        var main = mixerEffect.main;
        main.startColor = mixerColor;
    }
}