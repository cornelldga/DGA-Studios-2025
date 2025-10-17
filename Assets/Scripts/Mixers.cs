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

    public void OnMixer(InputAction.CallbackContext context)
    {
        if (!limeMixed && !pimientoMixed && !gingerMixed && !ciderMixed)
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
            Debug.Log("Ginger");
            pimientoMixed = false;
            gingerMixed = true;
            NoMixer();
            MixGinger();
        }
        else if (gingerMixed)
        {
            Debug.Log("Cider");
            gingerMixed = false;
            ciderMixed = true;
            NoMixer();
            mixCider();
        }
        else if (ciderMixed)
        {
            Debug.Log("None");
            ciderMixed = false;
            NoMixer();
        }
    }

    private void NoMixer()
    {
        pm.ResetCooldown();
        pm.ResetDamageSens();
        pm.ResetSpeed();
        pm.ResetCooldown();
        pm.ResetSpeed();
        pm.ResetDamageMod();
        pm.ResetAccuracyMod();
        pm.SetDestroyBulletsOn();
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

    private void MixGinger()
    {
        pm.SetDamageMod(gingerValue);
        pm.SetDestroyBulletsOn();
    }

    private void mixCider()
    {
        pm.SetSpeedMod(ciderValueSpeed);
        pm.SetAccuracyMod(ciderValueAccuracy);
    }
}
