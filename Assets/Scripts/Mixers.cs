using UnityEngine;
using UnityEngine.InputSystem;

public class Mixers : MonoBehaviour
{
    private PlayerInputActions playerControls;
    private InputAction mixer;

    [SerializeField] PlayerController playerController;
    [SerializeField] PlayerProjectile playerProjectile;

    [SerializeField] float limeJuiceValue;

    private bool limeMixed = false;

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
        if (!limeMixed)
        {
            limeMixed = true;
            playerProjectile.setCooldownMod(limeJuiceValue);
            playerController.applyStatus(limeJuiceValue,1);
        }
        else
        {
            limeMixed = false;
            playerProjectile.setCooldownMod(1);
            playerController.applyStatus(1,1);
        }
    }
}
