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

    Dictionary<BaseType, Button> baseToButton = new Dictionary<BaseType, Button>();
    Dictionary<MixerType, Button> mixerToButton = new Dictionary<MixerType, Button>();

    public BaseType[] GetEquippedBases() => equippedBases;
    public MixerType[] GetEquippedMixers() => equippedMixers;
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
    }

    /// <summary>
    /// Swaps the item out in a slot
    /// </summary>
    /// <param name="slot"></param>
    private void SelectSlot(Image slot)
    {
        if (highlighted==null)
        {
            // nothing selected
            slot.color = Color.lightGreen;
            highlighted = slot;
        } else
        {
            // something selected
            highlighted.color = Color.white;
            if (highlighted != slot)
            {
                // they are the same, then unselect
                highlighted = null;
            } else
            {
                // they are different
                slot.color = Color.lightGreen;
                highlighted = slot;

            }
            highlighted = null;
        }
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
        if (highlighted!=null & !mixer)
        {
            if (highlighted==baseSlotOne)
            {
                index = 0;
            } else
            {
                index = 1;
            }
            BaseType swappedBase  = GameManager.Instance.player.SwapBaseSlot(index,(BaseType)baseType);
            baseToButton[swappedBase].interactable = true;
            baseToButton[(BaseType)baseType].interactable = false;
            highlighted.color = Color.white;
            highlighted.sprite = baseToButton[(BaseType)baseType].image.sprite;
            highlighted = null;
            foreach (var button in baseButtons)
            {
                button.image.color = Color.white;
            }
        }
    }

    /// <summary>
    /// Choose a mixer to be equipped at the selected index
    /// </summary>
    public void ChooseMixer(int mixerType)
    {
        if (highlighted!=null && mixer)
        {
            if (highlighted==mixerSlotOne)
            {
                index = 0;
            } else
            {
                index = 1;
            }
            MixerType swappedMixer = GameManager.Instance.player.SwapMixerSlot(index, (MixerType)mixerType);
            mixerToButton[swappedMixer].interactable = true;
            mixerToButton[(MixerType)mixerType].interactable = false;
            highlighted.color = Color.white;
            highlighted.sprite = mixerToButton[(MixerType)mixerType].image.sprite;
            highlighted = null;
            foreach (var button in mixerButtons)
            {
                button.image.color = Color.white;
            }
        }
    }
}