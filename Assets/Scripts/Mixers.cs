using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mixers : MonoBehaviour
{
    [SerializeField] PlayerManager pm;
    [SerializeField] BossManager bm;
    [SerializeField] PlayerInventory playerInventory;
    [SerializeField] WhipCollisionHandler wch;
    [SerializeField] float limeJuiceValue;
    [SerializeField] float pimientoValue;
    [SerializeField] float gingerValueProjDamage;
    [SerializeField] float gingerValueWhipDamage;
    [SerializeField] float ciderValueSpeed;
    [SerializeField] float ciderValueAccuracy;

    private PlayerInventory.MixerType currentMixer = PlayerInventory.MixerType.None;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentMixer = PlayerInventory.MixerType.None;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (currentMixer != playerInventory.GetEquippedMixer())
        {
            currentMixer = playerInventory.GetEquippedMixer();
            print(currentMixer);
            switch (currentMixer)
            {
                case PlayerInventory.MixerType.None:
                    NoMixer();
                    break;

                case PlayerInventory.MixerType.Lime:
                    NoMixer();
                    MixLime();
                    break;

                case PlayerInventory.MixerType.Pimiento:
                    NoMixer();
                    MixPimiento();
                    break;

                case PlayerInventory.MixerType.Ginger:
                    NoMixer();
                    MixGinger();
                    break;

                case PlayerInventory.MixerType.Cider:
                    NoMixer();
                    MixCider();
                    break;
            }
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
        wch.ResetDamageMod();
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
        pm.SetDamageMod(gingerValueProjDamage);
        wch.SetDamageMod((int)gingerValueWhipDamage);
    }

    private void MixCider()
    {
        pm.SetSpeedMod(ciderValueSpeed);
        pm.SetAccuracyMod(ciderValueAccuracy);
    }
}
