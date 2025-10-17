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

    private InputAction fire;
    private InputAction change;

    private float angle;

    private float fireCooldown = 0;
    
    //Modifier for how long until next shot can be fired
    private float cooldownMod = 1;

    //Modifier for how much damage the projectile does to enemies
    private float damageMod = 1;

    //Modifier for how wide the range of possible directions the projectile goes in is
    private float accuracyMod = 1;

    //Whether projectile should destroy enemy bullets or not
    private bool destroyBullets = false;


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
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
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

    public void setDamageMod(float mod)
    {
        damageMod = mod;
    }

    public void setAccuracyMod(float mod)
    {
        accuracyMod = mod;
    }

    public void setDestroyBullets(bool ginger)
    {
        destroyBullets = ginger;
    }

    void ShootBeer()
    {
        Quaternion fireDirection = Quaternion.Euler(0f, 0f, angle + Random.Range(-beerPrefab.getAccuracy() * accuracyMod, beerPrefab.getAccuracy() * accuracyMod));
        Beer beerProj = Instantiate(beerPrefab, gameObject.transform.position, fireDirection);
        fireCooldown = beerProj.getCooldown() * cooldownMod;
    }

    void ShootGin()
    {
        Quaternion fireDirection = Quaternion.Euler(0f, 0f, angle + Random.Range(-ginPrefab.getAccuracy() * accuracyMod, ginPrefab.getAccuracy() * accuracyMod));
        Gin ginProj = Instantiate(ginPrefab, gameObject.transform.position, fireDirection);
        fireCooldown = ginProj.getCooldown() * cooldownMod;
    }

    void ShootWhiskey()
    {
        Quaternion fireDirection = Quaternion.Euler(0f, 0f, angle + Random.Range(-whiskeyPrefab.getAccuracy() * accuracyMod, whiskeyPrefab.getAccuracy() * accuracyMod));
        Whiskey whiskeyProj = Instantiate(whiskeyPrefab, gameObject.transform.position, fireDirection);
        fireCooldown = whiskeyProj.getCooldown() * cooldownMod;
    }

    void ShootWine()
    {
        Quaternion fireDirection = Quaternion.Euler(0f, 0f, angle + Random.Range(-winePrefab.getAccuracy() * accuracyMod, winePrefab.getAccuracy() * accuracyMod));
        Wine wineProj = Instantiate(winePrefab, gameObject.transform.position, fireDirection);
        fireCooldown = wineProj.getCooldown() * cooldownMod;
    }

}
