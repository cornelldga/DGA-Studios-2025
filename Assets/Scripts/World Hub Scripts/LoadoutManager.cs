using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadoutManager : MonoBehaviour
{
    [SerializeField] private Button firstMixer;
    [SerializeField] private Button secondMixer;
    [SerializeField] private Button thirdMixer;
    [SerializeField] private Button fourthMixer;
    [SerializeField] private Button firstBase;
    [SerializeField] private Button secondBase;
    [SerializeField] private Button thirdBase;
    [SerializeField] private Button fourthBase;
    [SerializeField] private Sprite baseSlotOne;
    [SerializeField] private Sprite baseSlotTwo;
    [SerializeField] private Sprite mixerSlotOne;
    [SerializeField] private Sprite mixerSlotTwo;
    private Sprite currentSlot;

    public void SelectMixer(Button buttonSelected)
    {
        MixerType mixer;
        if (buttonSelected == firstMixer)
        {
            mixer = MixerType.Cider;
            firstMixer.gameObject.SetActive(false);
            currentSlot = firstMixer.GetComponent<Sprite>();
        }
        else if (buttonSelected == secondMixer)
        {
            mixer = MixerType.Ginger;
            secondMixer.gameObject.SetActive(false);
            currentSlot = secondMixer.GetComponent<Sprite>();
        }
        else if (buttonSelected == thirdMixer)
        {
            mixer = MixerType.Lime;
            thirdMixer.gameObject.SetActive(false);
            currentSlot = thirdMixer.GetComponent<Sprite>();
        }
        else
        {
            mixer = MixerType.Pimiento;
            fourthMixer.gameObject.SetActive(false);
            currentSlot = fourthMixer.GetComponent<Sprite>();
        }
        // change it from just zero?
        GameManager.Instance.player.SwapMixerSlot(0, mixer);
    }

    public void SelectBase(Button buttonSelected)
    {
        BaseType baseType;
        if (buttonSelected == firstBase)
        {
            baseType = BaseType.Beer;
            firstBase.gameObject.SetActive(false);
            currentSlot = firstBase.GetComponent<Sprite>();
        }
        else if (buttonSelected == secondBase)
        {
            baseType = BaseType.Gin;
            secondBase.gameObject.SetActive(false);
            currentSlot = secondBase.GetComponent<Sprite>();
        }
        else if (buttonSelected == thirdBase)
        {
            baseType = BaseType.Whiskey;
            thirdBase.gameObject.SetActive(false);
            currentSlot = thirdBase.GetComponent<Sprite>();
        }
        else
        {
            baseType = BaseType.Wine;
            fourthBase.gameObject.SetActive(false);
            currentSlot = fourthBase.GetComponent<Sprite>();
        }
        // change it from just zero?
        GameManager.Instance.player.SwapBaseSlot(0, baseType);
    }

    public void Slot(Sprite slot)
    {
        if (slot==baseSlotOne)
        {
            currentSlot = baseSlotOne;
        } else if (slot==baseSlotTwo)
        {
            currentSlot = baseSlotTwo;
        } else if (slot==mixerSlotOne)
        {
            currentSlot = mixerSlotOne;
        } else
        {
            currentSlot = mixerSlotTwo;
        }
    }

}
