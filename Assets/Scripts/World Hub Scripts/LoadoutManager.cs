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

    Dictionary<BaseType, Button> baseToButton = new Dictionary<BaseType, Button>();
    Dictionary<MixerType, Button> mixerToButton = new Dictionary<MixerType, Button>();

    public BaseType[] GetEquippedBases() => equippedBases;
    public MixerType[] GetEquippedMixers() => equippedMixers;

    private List<BaseType> lockedBases = new List<BaseType>();
    private List<MixerType> lockedMixers = new List<MixerType>();

   [SerializeField] GameObject rows;
   [SerializeField] GameObject text;
   [SerializeField] GameObject baseOptions;
   [SerializeField] GameObject mixerOptions;

   [Tooltip("Base Icons")]
   [SerializeField] Sprite[] baseIcons;
   [Tooltip("Mixer Icons")]
   [SerializeField] Sprite[] mixerIcons;

    [Tooltip("Locked Icons")]
    [SerializeField] List<GameObject> baseChains;
    [SerializeField] List<GameObject> mixerChains;


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
        lockedBases = Enum.GetValues(typeof(BaseType))
                  .Cast<BaseType>()
                  .ToList();

        lockedMixers = Enum.GetValues(typeof(MixerType))
                        .Cast<MixerType>()
                        .ToList();

        int progression = PlayerPrefs.GetInt("progression", 0);

        // Default starter items
        lockedBases.Remove(BaseType.Beer);
        lockedBases.Remove(BaseType.Whiskey);
        lockedMixers.Remove(MixerType.Cider);
        lockedMixers.Remove(MixerType.Ginger);
        Debug.Log(progression);
        // Beat Drover
        if (progression > 1)
        {
            lockedBases.Remove(BaseType.Wine);
            baseChains[0].SetActive(false);
        }

        // Beat Julius
        if (progression > 2)
        {
            lockedMixers.Remove(MixerType.Lime);
            mixerChains[0].SetActive(false);
        }

        // Beat Ace & Mirage
        if (progression > 3)
        {
            lockedBases.Remove(BaseType.Gin);
            baseChains[1].SetActive(false);
        }

        // Beat Ash
        if (progression > 4)
        {
            lockedMixers.Remove(MixerType.Pimiento);
            mixerChains[1].SetActive(false);
        }
            RefreshBaseButtons();
            RefreshMixerButtons();
    }

    public void setLoadout(int slot)
    {
        text.SetActive(false);
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
            bool isLocked = lockedBases.Contains(pair.Key);
            pair.Value.interactable = !isLocked;
        }
    }

    void RefreshMixerButtons()
    {
        foreach (var pair in mixerToButton)
        {
            bool isLocked = lockedMixers.Contains(pair.Key);

            pair.Value.interactable = !isLocked;
        }
    }
    
    /// <summary>
    /// Choose a base to be equipped at the selected index
    /// </summary>
    public void Choose(int type)
    {
        if (mixer)
        {
            MixerType newMixer = (MixerType)type;

            if (!Array.Exists(equippedMixers, m => m == newMixer))
            {
                GameManager.Instance.player.SwapMixerSlot(slotNumber, newMixer);

                equippedMixers[slotNumber] = newMixer;

                currentSlot.sprite = mixerIcons[type];

                RefreshMixerButtons();
            }
        }
        else
        {
            BaseType newBase = (BaseType)type;

            if (!Array.Exists(equippedBases, b => b == newBase))
            {
                GameManager.Instance.player.SwapBaseSlot(slotNumber, newBase);

                equippedBases[slotNumber] = newBase;

                currentSlot.sprite = baseIcons[type];

                RefreshBaseButtons();
            }
        }
    }
}