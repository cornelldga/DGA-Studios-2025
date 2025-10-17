using UnityEngine;
using UnityEngine.InputSystem;

public class Mixers : MonoBehaviour
{
    private PlayerInputActions playerControls;
    private InputAction mixer;

    [SerializeField] PlayerController playerController;
    [SerializeField] PlayerProjectile playerProjectile;

    [SerializeField] float limeJuiceValue;
    [SerializeField] float pimientoValue;
    [SerializeField] float gingerValue;
    [SerializeField] float ciderValueSpeed;
    [SerializeField] float ciderValueAccuracy;

    private bool limeMixed = false;
    private bool pimientoMixed = false;
    private bool gingerMixed = false;
    private bool ciderMixed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        playerControls = new PlayerInputActions();
        mixer = playerControls.Player.Mixer;
        mixer.Enable();
    }
    private void OnDisable()
    {
        mixer.Disable();
    }

    void OnMixer()
    {
        if (!limeMixed && !pimientoMixed && !gingerMixed && !ciderMixed)
        {
            Debug.Log("Lime");
            limeMixed = true;
            noMixer();
            mixLime();
        }
        else if (limeMixed)
        {
            Debug.Log("Pimiento");
            limeMixed = false;
            pimientoMixed = true;
            noMixer();
            mixPimiento();
        }
        else if (pimientoMixed)
        {
            Debug.Log("Ginger");
            pimientoMixed = false;
            gingerMixed = true;
            noMixer();
            mixGinger();
        }
        else if (gingerMixed)
        {
            Debug.Log("Cider");
            gingerMixed = false;
            ciderMixed = true;
            noMixer();
            mixCider();
        }
        else if (ciderMixed)
        {
            Debug.Log("None");
            ciderMixed = false;
            noMixer();
        }
    }

    private void noMixer()
    {
        playerProjectile.setCooldownMod(1);
        playerController.setSpeedMod(1);
        playerProjectile.setDamageMod(1);
        playerProjectile.setAccuracyMod(1);
        playerProjectile.setDestroyBullets(false);
    }

    private void mixLime()
    {
        playerProjectile.setCooldownMod(limeJuiceValue);
        playerController.setSpeedMod(limeJuiceValue);
    }

    private void mixPimiento()
    {
        playerProjectile.setDamageMod(pimientoValue);
    }

    private void mixGinger()
    {
        playerProjectile.setDamageMod(gingerValue);
        playerProjectile.setDestroyBullets(true);
    }

    private void mixCider()
    {
        playerController.setSpeedMod(ciderValueSpeed);
        playerProjectile.setAccuracyMod(ciderValueAccuracy);
    }
}
