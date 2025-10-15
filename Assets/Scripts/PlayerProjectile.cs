using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
    private float damageMod = 1;


    [SerializeField] Beer beerPrefab;
    [SerializeField] Gin ginPrefab;
    [SerializeField] Whiskey whiskeyPrefab;
    [SerializeField] Wine winePrefab;

    [SerializeField] float changeCooldown;

    [SerializeField] TextMeshProUGUI projText;

    private PlayerInputActions playerControls;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isBeer = true;
        projText.text = "Beer";
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
                ShootWine();
            }
        }
    }

    void OnChange()
    {
        if (isBeer)
        {
            isBeer = false;
            isGin = true;
            projText.text = "Gin";
        }
        else if (isGin)
        {
            isGin = false;
            isWhiskey = true;
            projText.text = "Whiskey";
        }
        else if (isWhiskey)
        {
            isWhiskey = false;
            isWine = true;
            projText.text = "Wine";
        }
        else if (isWine){
            isWine = false;
            isBeer = true;
            projText.text = "Beer";
        }
        fireCooldown = changeCooldown;
    }

    public void setCooldownMod(float mod)
    {
        cooldownMod = mod;
    }

    public void SetDamageMod(float mod)
    {
        damageMod = mod;
    }

    void ShootBeer()
    {
          Beer beerProj = Instantiate(beerPrefab, gameObject.transform.position, fireDirection);
          beerProj.SetProjectile(speedMod, lifetimeMod);
          fireCooldown = beerProj.getCooldown() * cooldownMod;
    }

    void ShootGin()
    {
          Gin ginProj = Instantiate(ginPrefab, gameObject.transform.position, fireDirection);
          ginProj.SetProjectile(speedMod, lifetimeMod);
          fireCooldown = ginProj.getCooldown() * cooldownMod;
    }

    void ShootWhiskey()
    {
        Whiskey whiskeyProj = Instantiate(whiskeyPrefab, gameObject.transform.position, fireDirection);
        whiskeyProj.SetProjectile(speedMod, lifetimeMod);
        fireCooldown = whiskeyProj.getCooldown() * cooldownMod;
    }

    void ShootWine()
    {
        Wine wineProj = Instantiate(winePrefab, gameObject.transform.position, fireDirection);
        wineProj.SetProjectile(speedMod, lifetimeMod);
        fireCooldown = wineProj.getCooldown() * cooldownMod;
    }

}
