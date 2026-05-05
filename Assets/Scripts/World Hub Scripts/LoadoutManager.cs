using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutManager : MonoBehaviour
{
    [SerializeField] BaseType[] equippedBases;
    [SerializeField] MixerType[] equippedMixers;
    [Space(10)]
    [Tooltip("Index of the base/mixer to be swapped")]
    int index;
    [SerializeField] List<Button> baseButtons;
    [SerializeField] List<Button> mixerButtons;


    [Tooltip("Equipped slots")]
    [SerializeField] Image baseSlotOne;
    [SerializeField] Image baseSlotTwo;
    [SerializeField] Image mixerSlotOne;
    [SerializeField] Image mixerSlotTwo;
    private Image currentSlot;
    private int slotNumber;
    private bool mixer;
    private 

    Dictionary<BaseType, Button> baseToButton = new Dictionary<BaseType, Button>();
    Dictionary<MixerType, Button> mixerToButton = new Dictionary<MixerType, Button>();

    public BaseType[] GetEquippedBases() => equippedBases;
    public MixerType[] GetEquippedMixers() => equippedMixers;

    private List<BaseType> lockedBases = new List<BaseType>();
    private List<MixerType> lockedMixers = new List<MixerType>();

   [SerializeField] GameObject rows;
   [SerializeField] GameObject baseOptions;
   [SerializeField] GameObject mixerOptions;

   [Tooltip("Base Icons")]
   [SerializeField] Sprite[] baseIcons;
   [Tooltip("Mixer Icons")]
   [SerializeField] Sprite[] mixerIcons;

    [Tooltip("Locked Icons")]
    [SerializeField] GameObject[] baseChains;
    [SerializeField] GameObject[] mixerChains;


    /// <summary>
    /// Set up the dictionaries
    /// </summary>
    private void Awake()
    {
        int index = 0;
        foreach(var button in baseButtons)
        {
            baseToButton[(BaseType)index] = button;
            index++;
        }
        index = 0;
        foreach(var button in mixerButtons)
        {
            mixerToButton[(MixerType)index] = button;
            index++;
        }
        index = 0;
        foreach (BaseType baseType in equippedBases)
        {
            baseToButton[baseType].interactable = false;
            // set base slot images
            if (index==0)
            {
                baseSlotOne.sprite = baseIcons[(int) baseType];
            } else
            {
                baseSlotTwo.sprite = baseIcons[(int) baseType];
            }
            index++;
        }
        index = 0;
        foreach (MixerType mixerType in equippedMixers)
        {
            mixerToButton[mixerType].interactable = false;
            // set mixer slot images
            if (index==0)
            {
                mixerSlotOne.sprite = mixerIcons[(int) mixerType];
            } else
            {
                mixerSlotTwo.sprite = mixerIcons[(int) mixerType];
            }
            index++;
        }
        lockedBases.Clear();
        lockedMixers.Clear();
        if (PlayerPrefs.GetInt("progression",0)>1)
        {
            // Beat Drover
            lockedBases.Add(BaseType.Wine);
        }
        if (PlayerPrefs.GetInt("progression",0)>2)
        {
            // Beat Julius
            lockedMixers.Add(MixerType.Lime);
        }
        if (PlayerPrefs.GetInt("progression",0)>3)
        {
            // Beat Ace & Mirage
            lockedBases.Add(BaseType.Gin);
        }
        if (PlayerPrefs.GetInt("progression",0)>4)
        {
            // Beat Ash
            lockedMixers.Add(MixerType.Pimiento);
        }
        RefreshBaseButtons();
        RefreshMixerButtons();
    }

    public void setLoadout(int slot)
    {
        rows.SetActive(true);
        if (slot==1)
        {
            mixerOptions.SetActive(false);
            baseOptions.SetActive(true);
            currentSlot = baseSlotOne;
            slotNumber = 0;
            mixer = false;
        } else if (slot==2)
        {
            mixerOptions.SetActive(false);
            baseOptions.SetActive(true);
            currentSlot = baseSlotTwo;
            slotNumber = 1;
            mixer = false;
        } else if (slot==3)
        {
            baseOptions.SetActive(false);
            mixerOptions.SetActive(true);
            currentSlot = mixerSlotOne;
            slotNumber = 0;
            mixer = true;
        } else
        {
            baseOptions.SetActive(false);
            mixerOptions.SetActive(true);
            currentSlot = mixerSlotTwo;
            slotNumber = 1;
            mixer = true;
        }
    }


    /// <summary>
    /// Close the loadout manager
    /// </summary>
    public void Close()
    {
        GameManager.Instance.ToggleLoadoutManager(false);
    }

    void RefreshBaseButtons()
    {
        foreach (var pair in baseToButton)
        {
            bool isEquipped = equippedBases.Contains(pair.Key);
            bool isUnlocked = unlockedBases.Contains(pair.Key);
            pair.Value.interactable = isUnlocked && !isEquipped;
        }
    }

    void RefreshMixerButtons()
    {
        foreach (var pair in mixerToButton)
        {
            bool isEquipped = equippedMixers.Contains(pair.Key);
            bool isUnlocked = unlockedMixers.Contains(pair.Key);

            pair.Value.interactable = isUnlocked && !isEquipped;
        }
    }
    
    /// <summary>
    /// Choose a base to be equipped at the selected index
    /// </summary>
    public void Choose(int type)
    {
        if (mixer)
        {
            if (!Array.Exists(equippedMixers, name => name == (MixerType)type)) {
                MixerType swappedMixer = GameManager.Instance.player.SwapMixerSlot(slotNumber, (MixerType)type);
                currentSlot.sprite =  mixerIcons[type];
                RefreshMixerButtons();
                equippedMixers[slotNumber] = swappedMixer;
            }
        } else
        {
            if (!Array.Exists(equippedBases, name => name == (BaseType)type)) {
            BaseType swappedBase = GameManager.Instance.player.SwapBaseSlot(slotNumber,(BaseType)type);
            currentSlot.sprite =  baseIcons[type];
            RefreshBaseButtons();
            equippedBases[slotNumber] = swappedBase;
            }
        }
    }
}