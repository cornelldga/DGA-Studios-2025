using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.IO.Pipes;

public class PlayerProjectile : MonoBehaviour
{
    private bool isBeer = false;
    private bool isGin = false;
    private bool isWhiskey = false;
    private bool isWine = false;

    private float angle;

    private float fireCooldown = 0;
    
    //Modifier for how long until next shot can be fired
    private float cooldownMod = 1;

    //Modifier for how much damage the projectile does to enemies
    private float damageMod = 1;

    //Modifier for how wide the range of possible directions the projectile goes in is
    private float accuracyMod = 1;

    //Modifier for how fast a projectile moves
    private float speedMod = 1;

    //Modifier for how long a projectile lasts
    float lifetimeMod = 1;

    [SerializeField] PlayerInventory playerInventory;
    [SerializeField] Beer beerPrefab;
    [SerializeField] Gin ginPrefab;
    [SerializeField] Whiskey whiskeyPrefab;
    [SerializeField] Wine winePrefab;

    [SerializeField] float changeCooldown;

    private PlayerInputActions playerControls;
    private PlayerInventory.BaseType currentBase;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentBase = playerInventory.GetEquippedBase();
    }

    // Update is called once per frame
    void Update()
    {
        fireCooldown -= Time.deltaTime;
        if (Input.GetMouseButton(0) && fireCooldown <= 0)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mouseWorldPos.z = 0f;
            Vector3 direction = mouseWorldPos - transform.position;
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            currentBase = playerInventory.GetEquippedBase();
            switch (currentBase)
            {
            case PlayerInventory.BaseType.Beer:
                ShootBeer();
                break;
            case PlayerInventory.BaseType.Gin:
                ShootGin();
                break;
            case PlayerInventory.BaseType.Whiskey:
                ShootWhiskey();
                break;
            case PlayerInventory.BaseType.Wine:
                ShootWine();
                break;
            case PlayerInventory.BaseType.None:
                break;
            }
        }
    }


    public void setCooldownMod(float mod)
    {
        cooldownMod = mod;
    }

    public void SetDamageMod(float mod)
    {
        damageMod = mod;
    }

    public void setAccuracyMod(float mod)
    {
        accuracyMod = mod;
    }

    void ShootBeer()
    {
        Quaternion fireDirection = Quaternion.Euler(0f, 0f, angle + Random.Range(-beerPrefab.getAccuracy() * accuracyMod, beerPrefab.getAccuracy() * accuracyMod));
        Beer beerProj = Instantiate(beerPrefab, gameObject.transform.position, fireDirection);
        beerProj.SetDamageMod((int)damageMod);
        fireCooldown = beerProj.getCooldown() * cooldownMod;
    }

    void ShootGin()
    {
        Quaternion fireDirection = Quaternion.Euler(0f, 0f, angle + Random.Range(-ginPrefab.getAccuracy() * accuracyMod, ginPrefab.getAccuracy() * accuracyMod));
        Gin ginProj = Instantiate(ginPrefab, gameObject.transform.position, fireDirection);
        ginProj.SetDamageMod((int)damageMod);
        fireCooldown = ginProj.getCooldown() * cooldownMod;
    }

    void ShootWhiskey()
    {
        Quaternion fireDirection = Quaternion.Euler(0f, 0f, angle + Random.Range(-whiskeyPrefab.getAccuracy() * accuracyMod, whiskeyPrefab.getAccuracy() * accuracyMod));
        Whiskey whiskeyProj = Instantiate(whiskeyPrefab, gameObject.transform.position, fireDirection);
        whiskeyProj.SetDamageMod((int)damageMod);
        fireCooldown = whiskeyProj.getCooldown() * cooldownMod;
    }

    void ShootWine()
    {
        Quaternion fireDirection = Quaternion.Euler(0f, 0f, angle + Random.Range(-winePrefab.getAccuracy() * accuracyMod, winePrefab.getAccuracy() * accuracyMod));
        Wine wineProj = Instantiate(winePrefab, gameObject.transform.position, fireDirection);
        wineProj.SetDamageMod((int)damageMod);
        fireCooldown = wineProj.getCooldown() * cooldownMod;
    }

}
