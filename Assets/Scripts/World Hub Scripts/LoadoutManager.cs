using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutManager : MonoBehaviour
{
    [SerializeField] BaseType[] equippedBases;
    [SerializeField] MixerType[] equippedMixers;
    [Space(10)]
    [Tooltip("Index of the base/mixer to be swapped")]
    [SerializeField] GameObject baseGrid;
    [SerializeField] GameObject mixerGrid;
    [SerializeField] List<Button> baseButtons;
    [SerializeField] List<Button> mixerButtons;
    [SerializeField] List<GameObject> baseChains;
    [SerializeField] List<GameObject> mixerChains;
    [SerializeField] Image baseEquipImage1;
    [SerializeField] Image baseEquipImage2;
    [SerializeField] Image mixerEquipImage1;
    [SerializeField] Image mixerEquipImage2;
    [SerializeField] float selectScale;

    private int slotIndex = -1;
    int baseIndex = -1;
    int mixerIndex = -1;

    public BaseType[] GetEquippedBases() => equippedBases;
    public MixerType[] GetEquippedMixers() => equippedMixers;

    private void OnEnable()
    {
        baseGrid.SetActive(false);
        mixerGrid.SetActive(false);
    }


    private void Start()
    {
        int progression = PlayerPrefs.GetInt("progression", 0);
        // Beat Drover
        if (progression > 1)
        {
            baseChains[0].SetActive(false);
            baseButtons[2].interactable = true;
        }

        // Beat Julius
        if (progression > 2)
        {
            mixerChains[0].SetActive(false);
            mixerButtons[2].interactable = true;
        }

        // Beat Ace & Mirage
        if (progression > 3)
        {
            baseChains[1].SetActive(false);
            baseButtons[3].interactable = true;
        }

        // Beat Ash
        if (progression > 4)
        {
            mixerChains[1].SetActive(false);
            mixerButtons[3].interactable = true;
        }

        SetLoadoutUI();
    }
    /// <summary>
    /// Sets the equip sprites and interactables of the current equiped loadout
    /// </summary>
    void SetLoadoutUI()
    {
        baseEquipImage1.sprite = baseButtons[(int)equippedBases[0]].image.sprite;
        baseButtons[(int)equippedBases[0]].interactable = false;
        baseEquipImage2.sprite = baseButtons[(int)equippedBases[1]].image.sprite;
        baseButtons[(int)equippedBases[1]].interactable = false;
        mixerEquipImage1.sprite = mixerButtons[(int)equippedMixers[0]].image.sprite;
        mixerButtons[(int)equippedMixers[0]].interactable = false;
        mixerEquipImage2.sprite = mixerButtons[(int)equippedMixers[1]].image.sprite;
        mixerButtons[(int)equippedMixers[1]].interactable = false;
    }


    /// <summary>
    /// Close the loadout manager
    /// </summary>
    public void Close()
    {
        GameManager.Instance.ToggleLoadoutManager(false);
    }
    /// <summary>
    /// Sets a slot index. If a base is not selected, increases the scale of all base buttons that are interactable
    /// </summary>
    /// <param name="slot"></param>
    public void ChooseBaseSlot(int slot)
    {
        slotIndex = slot;
        baseGrid.SetActive(true);
        mixerGrid.SetActive(false);
        if (baseIndex != -1)
        {
            SwapBase();
        }
        else
        {
            foreach (Button baseButton in baseButtons)
            {
                if (baseButton.interactable)
                {
                    baseButton.GetComponent<Transform>().localScale = Vector3.one * selectScale;
                }
            }
        }
    }
    /// <summary>
    /// Sets the baseIndex and checks if a slot index was chosen. If not, scale up the slot index
    /// </summary>
    /// <param name="baseType"></param>
    public void ChooseBase(int baseType)
    {
        baseIndex = baseType;
        if(slotIndex != -1)
        {
            SwapBase();
        }
        else
        {
            baseEquipImage1.GetComponent<Transform>().localScale = Vector3.one * selectScale;
            baseEquipImage2.GetComponent<Transform>().localScale = Vector3.one * selectScale;
        }

    }
    /// <summary>
    /// Swap the bases and toggle interaction. resets any scaling
    /// </summary>
    void SwapBase()
    {
        BaseType swappedBase = GameManager.Instance.player.SwapBaseSlot(slotIndex, (BaseType)baseIndex);

        if(slotIndex == 0)
        {
            baseEquipImage1.sprite = baseButtons[baseIndex].image.sprite;
        }
        else
        {
            baseEquipImage2.sprite = baseButtons[baseIndex].image.sprite;
        }

         baseButtons[baseIndex].interactable = false;
        baseButtons[(int)swappedBase].interactable = true;

        baseIndex = -1;
        slotIndex = -1;
        foreach (Button baseButton in baseButtons)
        {
            baseButton.GetComponent<Transform>().localScale = Vector3.one;
        }
        baseEquipImage1.GetComponent<Transform>().localScale = Vector3.one;
        baseEquipImage2.GetComponent<Transform>().localScale = Vector3.one ;
    }

    /// <summary>
    /// Sets the slot index. If a mixer is not selected, increases the scale of all mixer buttons that are interactable
    /// </summary>
    /// <param name="slot"></param>
    public void ChooseMixerSlot(int slot)
    {
        slotIndex = slot;
        mixerGrid.SetActive(true);
        baseGrid.SetActive(false);
        if (mixerIndex != -1)
        {
            SwapMixer();
        }
        else
        {
            foreach (Button mixerButton in mixerButtons)
            {
                if (mixerButton.interactable)
                {
                    mixerButton.GetComponent<Transform>().localScale = Vector3.one * selectScale;
                }
            }
        }
    }
    /// <summary>
    /// Sets the mixerIndex and checks if a slot index was chosen. If not, scale up the slot index
    /// </summary>
    /// <param name="mixerType"></param>
    public void ChooseMixer(int mixerType)
    {
        mixerIndex = mixerType;
        if (slotIndex != -1)
        {
            SwapMixer();
        }
        else
        {
            mixerEquipImage1.GetComponent<Transform>().localScale = Vector3.one * selectScale;
            mixerEquipImage2.GetComponent<Transform>().localScale = Vector3.one * selectScale;
        }

    }
    /// <summary>
    /// Swap the mixers and toggle interaction. resets any scaling
    /// </summary>
    void SwapMixer()
    {
        MixerType swappedMixer = GameManager.Instance.player.SwapMixerSlot(slotIndex, (MixerType)mixerIndex);

        if (slotIndex == 0)
        {
            mixerEquipImage1.sprite = mixerButtons[mixerIndex].image.sprite;
        }
        else
        {
            mixerEquipImage2.sprite = mixerButtons[mixerIndex].image.sprite;
        }

        mixerButtons[mixerIndex].interactable = false;
        mixerButtons[(int)swappedMixer].interactable = true;

        mixerIndex = -1;
        slotIndex = -1;
        foreach (Button mixerButton in mixerButtons)
        {
            mixerButton.GetComponent<Transform>().localScale = Vector3.one;
        }
        mixerEquipImage1.GetComponent<Transform>().localScale = Vector3.one;
        mixerEquipImage2.GetComponent<Transform>().localScale = Vector3.one;
    }
}