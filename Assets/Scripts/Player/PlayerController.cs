using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

/// <summary>
/// Controls the player
/// </summary>
public class PlayerController : MonoBehaviour

{
    [Header("Stats")]
    [SerializeField] float baseSpeed;
    private float speed;
    [SerializeField] int maxHealth;
    private int health;
    [SerializeField] float changeCooldownTime;
    private bool isAlive;

    [Header("Player Inventory")]
    [SerializeField] private MixerType[] equippedMixers;
    PlayerMixers playerMixers;
    [SerializeField] private BaseType[] equippedBases;
    PlayerBases playerBases;

    Rigidbody2D rb;
    float angle;
    Vector2 moveDirection;
    float fireCooldown;
    float changeCooldown;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMixers = GetComponent<PlayerMixers>();
        playerBases = GetComponent<PlayerBases>();
        // This should be set by the equipped mixer and not by the base stats
        // Introduces issue of checking equipped mixer first, then setting the player stats
        speed = baseSpeed;
        health = maxHealth;
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
        PlayerInputs();
    }
    void PlayerInputs()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector2(horizontal, vertical);
        if(changeCooldown <= 0)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                playerBases.SelectBase(equippedBases[0]);
                changeCooldown = changeCooldownTime;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                playerBases.SelectBase(equippedBases[1]);
                changeCooldown = changeCooldownTime;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                playerMixers.SelectMixer(equippedMixers[0]);
                playerMixers.GetMixer().ApplyMixer(this);
                changeCooldown = changeCooldownTime;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                playerMixers.SelectMixer(equippedMixers[1]);
                playerMixers.GetMixer().ApplyMixer(this);
                changeCooldown = changeCooldownTime;
            }
        }

        if (Input.GetMouseButton(0) && fireCooldown <= 0)
        {
            Fire();
        }
    }

    void Fire()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0f;
        Vector3 direction = mouseWorldPos - transform.position;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion fireDirection = Quaternion.Euler(0f, 0f, angle);
        // TODO: The position should be a transform where the player fires, not the center of the player
        Base baseDrink = Instantiate(playerBases.GetBase(), transform.position, fireDirection);
        playerMixers.GetMixer().ApplyMixer(baseDrink);
        
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


}