using System.Collections;
using System.Collections.Generic;
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
    private bool mixer;
    private 

    Dictionary<BaseType, Button> baseToButton = new Dictionary<BaseType, Button>();
    Dictionary<MixerType, Button> mixerToButton = new Dictionary<MixerType, Button>();

    public BaseType[] GetEquippedBases() => equippedBases;
    public MixerType[] GetEquippedMixers() => equippedMixers;

    private List<BaseType> unlockedBases = new List<BaseType>();
    private List<MixerType> unlockedMixers = new List<MixerType>();

   [SerializeField] GameObject rows;
   [SerializeField] GameObject baseOptions;
   [SerializeField] GameObject mixerOptions;

   [Tooltip("Base Icons")]
   [SerializeField] Sprite[] baseIcons;
   [Tooltip("Mixer Icons")]
   [SerializeField] Sprite[] mixerIcons;


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
        unlockedBases.Clear();
        unlockedMixers.Clear();
        // Default unlocked
        unlockedBases.Add(BaseType.Beer);
        unlockedBases.Add(BaseType.Whiskey);
        unlockedMixers.Add(MixerType.Ginger);
        unlockedMixers.Add(MixerType.Cider);
        if (PlayerPrefs.GetInt("progression",0)>1)
        {
            // Beat Drover
            unlockedBases.Add(BaseType.Wine);
        }
        if (PlayerPrefs.GetInt("progression",0)>2)
        {
            // Beat Julius
            unlockedMixers.Add(MixerType.Lime);
        }
        if (PlayerPrefs.GetInt("progression",0)>3)
        {
            // Beat Ace & Mirage
            unlockedBases.Add(BaseType.Gin);
        }
        if (PlayerPrefs.GetInt("progression",0)>4)
        {
            // Beat Ash
            unlockedMixers.Add(MixerType.Pimiento);
        }
    }

    public void setLoadout(int slot)
    {
        rows.SetActive(true);
        if (slot==1)
        {
            mixerOptions.SetActive(false);
            baseOptions.SetActive(true);
            currentSlot = baseSlotOne;
            mixer = false;
        } else if (slot==2)
        {
            mixerOptions.SetActive(false);
            baseOptions.SetActive(true);
            currentSlot = baseSlotTwo;
            mixer = false;
        } else if (slot==3)
        {
            baseOptions.SetActive(false);
            mixerOptions.SetActive(true);
            currentSlot = mixerSlotOne;
            mixer = true;
        } else
        {
            baseOptions.SetActive(false);
            mixerOptions.SetActive(true);
            currentSlot = mixerSlotTwo;
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
    
    /// <summary>
    /// Choose a base to be equipped at the selected index
    /// </summary>
    public void Choose(int type)
    {
        if (mixer)
        {
            MixerType swappedMixer = GameManager.Instance.player.SwapMixerSlot(index, (MixerType)type);
            currentSlot.sprite =  mixerIcons[type];
            mixerToButton[swappedMixer].interactable = true;
            mixerToButton[(MixerType)type].interactable = false;
        } else
        {
            BaseType swappedBase = GameManager.Instance.player.SwapBaseSlot(index,(BaseType)type);
            currentSlot.sprite =  baseIcons[type];
            baseToButton[swappedBase].interactable = true;
            baseToButton[(BaseType)type].interactable = false;
        }
    }
}