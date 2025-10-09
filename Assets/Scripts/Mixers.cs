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

    private bool limeMixed = false;
    private bool pimientoMixed = false;

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
        if (!limeMixed && !pimientoMixed)
        {
            Debug.Log("Lime");
            limeMixed = true;
            playerProjectile.setCooldownMod(limeJuiceValue);
            playerController.setSpeedMod(limeJuiceValue);
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
            Debug.Log("None");
            pimientoMixed = false;
            noMixer();
        }
    }

    private void noMixer()
    {
        playerProjectile.setCooldownMod(1);
        playerController.setSpeedMod(1);
        playerProjectile.setDamageMod(1);
    }

    private void mixLime()
    {
        playerProjectile.setCooldownMod(limeJuiceValue);
        playerController.ApplyStatus(1,limeJuiceValue);
    }

    private void mixPimiento()
    {
        playerProjectile.setDamageMod(pimientoValue);
    }
}
