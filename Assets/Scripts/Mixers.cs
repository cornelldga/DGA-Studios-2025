using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mixers : MonoBehaviour
{
    [SerializeField] PlayerManager pm;
    [SerializeField] PlayerInventory playerInventory;

    private InputAction mixer;
    [SerializeField] float limeJuiceValue;
    [SerializeField] float pimientoValue;
    [SerializeField] float gingerValue;
    [SerializeField] float ciderValueSpeed;
    [SerializeField] float ciderValueAccuracy;

    private PlayerInventory.MixerType currentMixer = PlayerInventory.MixerType.None;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        print(currentMixer);
        currentMixer = playerInventory.GetEquippedMixer();
          switch (currentMixer)
        {
            case PlayerInventory.MixerType.None:
                MixLime();
                break;

            case PlayerInventory.MixerType.Lime:
                MixPimiento();
                break;

            case PlayerInventory.MixerType.Pimiento:
                currentMixer = PlayerInventory.MixerType.Ginger;
                MixGinger();
                break;

            case PlayerInventory.MixerType.Ginger:
                currentMixer = PlayerInventory.MixerType.Cider;
                mixCider();
                break;

            case PlayerInventory.MixerType.Cider:
                currentMixer = PlayerInventory.MixerType.None;
                break;
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
