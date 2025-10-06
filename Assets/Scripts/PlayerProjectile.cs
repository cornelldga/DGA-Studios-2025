using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerProjectile : MonoBehaviour
{
    private bool isBeer = false;
    private bool isGin = false;
    private bool isWhiskey = false;
    private bool isWine = false;

    private InputAction fire;
    private InputAction change;

    private Quaternion fireDirection;

    private float fireCooldown = 0;

    private float cooldownMod = 1;
    private float speedMod = 1;
    private float lifetimeMod = 1;


    [SerializeField] Beer beerPrefab;
    [SerializeField] Gin ginPrefab;
    [SerializeField] Whiskey whiskeyPrefab;
    [SerializeField] Gin winePrefab;

    [SerializeField] float changeCooldown;

    private PlayerInputActions playerControls;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isBeer = true;
    }

    private void OnEnable()
    {
        playerControls = new PlayerInputActions();
        fire = playerControls.Player.Fire;
        fire.Enable();
        change = playerControls.Player.Change;
        change.Enable();
    }

    private void OnDisable()
    {
        fire.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        fireCooldown -= Time.deltaTime;
        if (fire.ReadValue<float>() > 0 && fireCooldown <= 0)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mouseWorldPos.z = 0f;
            Vector3 direction = mouseWorldPos - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            fireDirection = Quaternion.Euler(0f, 0f, angle);
            if (isBeer)
            {
                ShootBeer();
            }
            if (isGin)
            {
                ShootGin();
            }
            if (isWhiskey)
            {
                ShootWhiskey();
            }
            if (isWine)
            {

            }
        }
    }

    void OnChange()
    {
        if (isBeer)
        {
            isBeer = false;
            isGin = true;
        }
        else if (isGin)
        {
            isGin = false;
            isWhiskey = true;
        }
        else if (isWhiskey)
        {
            isWhiskey = false;
            isWine = true;
        }
        else if (isWine){
            isWine = false;
            isBeer = true;
        }
        fireCooldown = changeCooldown;
    }

    public void setCooldownMod(float mod)
    {
        cooldownMod = mod;
    }

    void ShootBeer()
    {
          Beer beerProj = Instantiate(beerPrefab, gameObject.transform.position, fireDirection);
          fireCooldown = beerProj.getCooldown() * cooldownMod;
    }

    void ShootGin()
    {
          Gin ginProj = Instantiate(ginPrefab, gameObject.transform.position, fireDirection);
          fireCooldown = ginProj.getCooldown() * cooldownMod;
    }

    void ShootWhiskey()
    {
        Whiskey whiskeyProj = Instantiate(whiskeyPrefab, gameObject.transform.position, fireDirection);
        fireCooldown = whiskeyProj.getCooldown() * cooldownMod;
    }

}
