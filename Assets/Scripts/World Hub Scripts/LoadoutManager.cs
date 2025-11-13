using UnityEngine;
using UnityEngine.UI;

public class LoadoutManager : MonoBehaviour
{
    [Header("The mixer + base buttons")]
    [SerializeField] private Button firstMixer;
    [SerializeField] private Button secondMixer;
    [SerializeField] private Button thirdMixer;
    [SerializeField] private Button fourthMixer;
    [SerializeField] private Button firstBase;
    [SerializeField] private Button secondBase;
    [SerializeField] private Button thirdBase;
    [SerializeField] private Button fourthBase;
    [Header("The inventory slot sprite renderers")]
    [SerializeField] private SpriteRenderer baseSlotOne;
    [SerializeField] private SpriteRenderer baseSlotTwo;
    [SerializeField] private SpriteRenderer mixerSlotOne;
    [SerializeField] private SpriteRenderer mixerSlotTwo;
    private SpriteRenderer currentSlot;

    /// <summary>
    /// Sets a mixer in a specific slot for the player and changes the image displayed in slot.
    /// <param name="buttonSelected">The Button that was clicked by the player.</param>
    /// </summary>
    public void SelectMixer(Button buttonSelected)
    {
        MixerType mixerType;
        Button button;
        if (buttonSelected == firstMixer)
        {
            mixerType = MixerType.Cider;
            button = firstMixer;
        } else if (buttonSelected == secondMixer)
        {
            mixerType = MixerType.Ginger;
            button = secondMixer;
        }
        else if (buttonSelected == thirdMixer)
        {
            mixerType = MixerType.Lime;
            button = thirdMixer;
        }
        else
        {
            mixerType = MixerType.Pimiento;
            button = fourthMixer;
        }
        button.gameObject.SetActive(false);
        currentSlot.sprite = button.GetComponent<Sprite>();
        if (currentSlot==mixerSlotOne)
        {
            GameManager.Instance.player.SwapMixerSlot(0, mixerType);
        } else if (currentSlot==mixerSlotTwo)
        {
            GameManager.Instance.player.SwapMixerSlot(1, mixerType);
        } else
        {
            Debug.Log("No mixer slot selected");
        }
    }

    /// <summary>
    /// Sets a base in a specific slot for the player and changes the image displayed in slot.
    /// <param name="buttonSelected">The Button that was clicked by the player.</param>
    /// </summary>
    public void SelectBase(Button buttonSelected)
    {
        BaseType baseType;
        Button button;
        if (buttonSelected == firstBase)
        {
            baseType = BaseType.Beer;
            button = firstBase;
        }
        else if (buttonSelected == secondBase)
        {
            baseType = BaseType.Gin;
            button = secondBase;
        }
        else if (buttonSelected == thirdBase)
        {
            baseType = BaseType.Whiskey;
            button = thirdBase;
        }
        else
        {
            baseType = BaseType.Wine;
            button = fourthBase;
        }
        button.gameObject.SetActive(false);
        currentSlot.sprite = button.GetComponent<Sprite>();
        if (currentSlot==baseSlotOne)
        {
            GameManager.Instance.player.SwapBaseSlot(0, baseType);
        } else if (currentSlot==baseSlotTwo)
        {
            GameManager.Instance.player.SwapBaseSlot(1, baseType);
        } else
        {
            Debug.Log("No base slot selected");
        }
    }

    /// <summary>
    /// When a slot is selected, changes current slot to the slot selected
    /// <param name="slot">The SpriteRenderer of the slot that was selected by the player.</param>
    /// </summary>
    public void Slot(SpriteRenderer slot)
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
