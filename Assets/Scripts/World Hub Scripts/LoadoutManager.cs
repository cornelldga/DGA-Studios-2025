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
    private Image highlighted;
    private bool mixer;
    private 

    Dictionary<BaseType, Button> baseToButton = new Dictionary<BaseType, Button>();
    Dictionary<MixerType, Button> mixerToButton = new Dictionary<MixerType, Button>();

    public BaseType[] GetEquippedBases() => equippedBases;
    public MixerType[] GetEquippedMixers() => equippedMixers;

    private List<BaseType> unlockedBases = new List<BaseType>();
    private List<MixerType> unlockedMixers = new List<MixerType>();

   [SerializeField] GameObject rows;
   [SerializeField] GameObject Bases;
   [SerializeField] GameObject Mixers;


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
                baseSlotOne.sprite = baseToButton[baseType].image.sprite;
            } else
            {
                baseSlotTwo.sprite = baseToButton[baseType].image.sprite;
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
                mixerSlotOne.sprite = mixerToButton[mixerType].image.sprite;
            } else
            {
                mixerSlotTwo.sprite = mixerToButton[mixerType].image.sprite;
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

    public void setLoadout(bool isMixer)
    {
        mixer = isMixer;
    }

    /// <summary>
    /// Swaps the item out in a slot
    /// </summary>
    /// <param name="slot"></param>
    private void SelectSlot(Image slot)
    {
        if (highlighted!=null)
        {
            // something already selected
            highlighted.color = Color.white;
            foreach (var button in mixerButtons)
            {
                button.image.color = Color.white;
            }
            foreach (var button in baseButtons)
            {
                button.image.color = Color.white;
            }
            if (highlighted == slot)
            {
                // they are the same, then unselect
                highlighted = null;
                return;
            }
        }
        slot.color = Color.lightGreen;
        highlighted = slot;
    }

    /// <summary>
    /// Highlights the base slot that is being changed and the available bases
    /// </summary>
    /// <param name="slot"></param>
    public void SelectBase(Image slot)
    {
        foreach (var button in baseButtons)
        {
            if (button.interactable==true)
            {
                button.image.color = Color.lightGreen;
                button.transform.localScale = Vector3.one * 1.2f;
            }
        }
        SelectSlot(slot);
        mixer = false;
    }

    /// <summary>
    /// Highlights the mixer slot that is being changed and the available mixers
    /// </summary>
    /// <param name="slot"></param>
    public void SelectMixer(Image slot)
    {
        foreach (var button in mixerButtons)
        {
            if (button.interactable==true)
            {
                button.image.color = Color.lightGreen;
                button.transform.localScale = Vector3.one * 1.2f;
            }
        }
        SelectSlot(slot);
        mixer = true;
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
    public void ChooseBase(int baseType)
    {
        if (highlighted==null || !mixer)
        {
            if (highlighted==null)
            {
                if (baseToButton[(BaseType)baseType].image.sprite==baseSlotOne.sprite)
                {
                    index=0;
                } else
                {
                    index=1;
                }
            } else if (highlighted==baseSlotOne)
            {
               index = 0; 
            } else
            {
                index = 1;
            }
            BaseType swappedBase = GameManager.Instance.player.SwapBaseSlot(index,(BaseType)baseType);
            baseToButton[swappedBase].interactable = true;
            baseToButton[(BaseType)baseType].interactable = false;
            if (highlighted!=null)
            {
                highlighted.color = Color.white;
                highlighted.sprite = baseToButton[(BaseType)baseType].image.sprite;
                highlighted = null;
            } else
            {
                if (index==0)
                {
                    baseSlotOne.sprite = baseToButton[(BaseType)baseType].image.sprite;
                } else
                {
                    baseSlotTwo.sprite = baseToButton[(BaseType)baseType].image.sprite;
                }
            }
            foreach (var button in baseButtons)
            {
                button.image.color = Color.white;
                button.transform.localScale = Vector3.one;
            }
        }
    }

    /// <summary>
    /// Choose a mixer to be equipped at the selected index
    /// </summary>
    public void ChooseMixer(int mixerType)
    {
        if (highlighted==null || mixer)
        {
            if (highlighted==null)
            {
                if (mixerToButton[(MixerType)mixerType].image.sprite==mixerSlotOne.sprite)
                {
                    index=0;
                } else
                {
                    index=1;
                }
            } else if (highlighted==mixerSlotOne)
            {
               index = 0; 
            } else
            {
                index = 1;
            }
            MixerType swappedMixer = GameManager.Instance.player.SwapMixerSlot(index, (MixerType)mixerType);
            mixerToButton[swappedMixer].interactable = true;
            mixerToButton[(MixerType)mixerType].interactable = false;
            if (highlighted!=null)
            {
                highlighted.color = Color.white;
                highlighted.sprite = mixerToButton[(MixerType)mixerType].image.sprite;
                highlighted = null;
            } else
            {
                if (index==0)
                {
                    mixerSlotOne.sprite = mixerToButton[(MixerType)mixerType].image.sprite;
                } else
                {
                    mixerSlotTwo.sprite = mixerToButton[(MixerType)mixerType].image.sprite;
                }
            }
            foreach (var button in mixerButtons)
            {
                button.image.color = Color.white;
                button.transform.localScale = Vector3.one;
            }
        }
    }
}