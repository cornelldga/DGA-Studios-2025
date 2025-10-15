using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mixers : MonoBehaviour
{
    [SerializeField] PlayerManager pm;
    private InputAction mixer;

    private PlayerInputActions playerControls;


    [SerializeField] float limeJuiceValue;
    [SerializeField] float pimientoValue;

    [SerializeField] bool limeMixed = false;
    [SerializeField] bool pimientoMixed = false;

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

    public void OnMixer(InputAction.CallbackContext context)
    {
        Debug.Log("Called");
        if (!limeMixed && !pimientoMixed)
        {
            Debug.Log("Lime");
            limeMixed = true;
            MixLime();
            pm.SetCooldownMod(limeJuiceValue);
            pm.SetSpeedMod(limeJuiceValue);
        }
        else if (limeMixed)
        {
            Debug.Log("Pimiento");
            limeMixed = false;
            pimientoMixed = true;
            NoMixer();
            MixPimiento();
        }
        else if (pimientoMixed)
        {
            Debug.Log("None");
            pimientoMixed = false;
            NoMixer();
        }
    }

    private void NoMixer()
    {
        pm.ResetCooldown();
        pm.ResetDamageSens();
        pm.ResetSpeed();
    }

    private void MixLime()
    {
        pm.SetCooldownMod(limeJuiceValue);
        pm.SetSpeedMod(limeJuiceValue);
    }

    private void MixPimiento()
    {
        pm.SetDamageMod(pimientoValue);
    }
}
