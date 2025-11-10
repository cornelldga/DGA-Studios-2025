using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

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
    [Tooltip("Percent damage dealt back from an enemy projectile")]
    public float whipBaseDamageMultiplier;
    [SerializeField] float changeCooldownTime;
    private bool isAlive;

    [Header("Player Inventory")]
    [SerializeField] private BaseType[] equippedBases;
    PlayerBases playerBases;
    [SerializeField] private MixerType[] equippedMixers;
    PlayerMixers playerMixers;

    [Header("Whip")]
    [SerializeField] Transform whipPivot;
    public Whip whip;
    [SerializeField] float whipCooldownTime;
    [SerializeField] float whipTime;

    //a magical number that I use to divide the offset of an angle from the angle it should move towards
    //this helps me make that micromovement I need to move the whip in a way that is less warped.
    //the bigger the number, the less we will adjust
    private float MAGIC_ADJUSTMENT_RATIO = 5f;

    Rigidbody2D rb;
    float angle;
    Vector2 moveDirection;
    float fireCooldown;
    float changeCooldown;
    float whipCooldown;
    private bool whipping;

    Base selectedBase;
    Mixer selectedMixer;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMixers = GetComponent<PlayerMixers>();
        playerBases = GetComponent<PlayerBases>();
        // This should be set by the equipped mixer and not by the base stats
        // Introduces issue of checking equipped mixer first, then setting the player stats
        speed = baseSpeed;
        health = maxHealth;
        whip.damageMultiplier = whipBaseDamageMultiplier;
        selectedBase = playerBases.GetBase(equippedBases[0]);
        selectedMixer = playerMixers.GetMixer(equippedMixers[0]);
        selectedMixer.ApplyMixer(this);

        isAlive = true;
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
        PlayerInputs();
    }
    /// <summary>
    /// Read the player's inputs
    /// </summary>
    void PlayerInputs()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector2(horizontal, vertical);
        if(changeCooldown <= 0)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                selectedBase = playerBases.GetBase(equippedBases[0]);
                changeCooldown = changeCooldownTime;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                selectedBase = playerBases.GetBase(equippedBases[1]);
                changeCooldown = changeCooldownTime;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                selectedMixer.RemoveMixer(this);
                selectedMixer = playerMixers.GetMixer(equippedMixers[0]);
                selectedMixer.ApplyMixer(this);
                changeCooldown = changeCooldownTime;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                selectedMixer.RemoveMixer(this);
                selectedMixer = playerMixers.GetMixer(equippedMixers[1]);
                selectedMixer.ApplyMixer(this);
                changeCooldown = changeCooldownTime;
            }
        }
        if (!whipping && Input.GetMouseButtonDown(1) && whipCooldown <= 0)
        {
            OnWhip();
            whipCooldown = whipCooldownTime;
        }

        if (Input.GetMouseButton(0) && fireCooldown <= 0)
        {
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
        Quaternion fireDirection = Quaternion.Euler(0f, 0f, angle);
        // TODO: The position should be a transform where the player fires, not the center of the player
        Base baseDrink = Instantiate(selectedBase, transform.position, fireDirection);
        selectedMixer.ApplyMixer(baseDrink);
        fireCooldown = baseDrink.cooldown;
    }

    /// <summary>
    /// Returns an adjusted angle that makes the whip's final location more accurate to where the user clicked
    /// </summary>
    public float AngleAdjustment(float originalAngle)
    {
        float finalAngle = originalAngle;
        float sign = originalAngle / Mathf.Abs(originalAngle);
        float adjustAmountAbs = 0;
        if (originalAngle >= -45 && originalAngle <= 45)
        {
            //adjust against 0
            adjustAmountAbs = Mathf.Abs(originalAngle);
        }
        else if (originalAngle <= -45 && originalAngle >= -135)
        {
            adjustAmountAbs = Mathf.Abs(90 - Mathf.Abs(originalAngle));
            //adjust against -90
        }
        else if (originalAngle <= 135 && originalAngle >= 45)
        {
            adjustAmountAbs = Mathf.Abs(90 - Mathf.Abs(originalAngle));
            //adjust against 90

        }
        else if (originalAngle >= 135 || originalAngle <= -135)
        {
            //adjust against 180
            adjustAmountAbs = 180 - Mathf.Abs(originalAngle);
        }

        if (Mathf.Abs(originalAngle) < 90)
        {
            //adjust towards 0
            finalAngle = originalAngle - (sign * adjustAmountAbs / MAGIC_ADJUSTMENT_RATIO);
        }
        else if (Mathf.Abs(originalAngle) > 90)
        {
            //adjust towards 180
            finalAngle = originalAngle + (sign * adjustAmountAbs / MAGIC_ADJUSTMENT_RATIO);
        }
        return finalAngle;
    }
    /// <summary>
    /// Rotates the whip towards the mouse and 
    /// </summary>
    public void OnWhip()
    {
        whipping = true;
        whip.gameObject.SetActive(true);
        //find angle between player and mouse
        //whipObject.transform.rotation = Quaternion.identity;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = 0f;
        Vector3 direction = mousePosition - whipPivot.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle = AngleAdjustment(angle);
        whipPivot.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        StartCoroutine(nameof(WhipTime));
    }

    IEnumerator WhipTime()
    {
        yield return new WaitForSeconds(whipTime);
        whipping = false;
        whip.gameObject.SetActive(false);
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

    public void TakeDamage(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            GameManager.Instance.LoseGame();
        }
    }
}