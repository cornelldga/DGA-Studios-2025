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
        foreach (BaseType baseType in equippedBases)
        {
            baseToButton[baseType].interactable = false;
        }
        foreach (MixerType mixerType in equippedMixers)
        {
            mixerToButton[mixerType].interactable = false;
        }
    }
    /// <summary>
    /// Sets the index of where bases and mixers will be swapped with the loadout
    /// </summary>
    /// <param name="index"></param>
    public void SelectIndex(int index)
    {
        this.index = index;
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

        BaseType swappedBase  = GameManager.Instance.player.SwapBaseSlot(index,(BaseType)baseType);
        baseToButton[swappedBase].interactable = true;
        baseToButton[(BaseType)baseType].interactable = false;
    }

    /// <summary>
    /// Choose a mixer to be equipped at the selected index
    /// </summary>
    public void ChooseMixer(int mixerType)
    {
        MixerType swappedMixer = GameManager.Instance.player.SwapMixerSlot(index, (MixerType)mixerType);
        mixerToButton[swappedMixer].interactable = true;
        mixerToButton[(MixerType)mixerType].interactable = false;
    }
}