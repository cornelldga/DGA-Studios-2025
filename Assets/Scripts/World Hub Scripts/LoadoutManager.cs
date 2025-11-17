using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutManager : MonoBehaviour
{
    [Header("The Mixer + Base Buttons")]
    [SerializeField] private Button beerButton;
    [SerializeField] private Button ginButton;
    [SerializeField] private Button whiskeyButton;
    [SerializeField] private Button wineButton;
    [SerializeField] private Button ciderButton;
    [SerializeField] private Button gingerButton;
    [SerializeField] private Button limeButton;
    [SerializeField] private Button pimientoButton;

    [Header("The Inventory Slot Sprite Renderers")]
    [SerializeField] private SpriteRenderer baseSlotOne;
    [SerializeField] private SpriteRenderer baseSlotTwo;
    [SerializeField] private SpriteRenderer mixerSlotOne;
    [SerializeField] private SpriteRenderer mixerSlotTwo;
    private SpriteRenderer currentSlot;
    private Dictionary<BaseType, Button> baseButtons;
    private Dictionary<MixerType, Button> mixerButtons;


    private void Start()
    {
        baseButtons = new Dictionary<BaseType, Button>()
        {
        { BaseType.Beer, beerButton },
        { BaseType.Gin, ginButton },
        { BaseType.Whiskey, whiskeyButton },
        { BaseType.Wine, wineButton }
    };
        mixerButtons = new Dictionary<MixerType, Button>()
        {
        { MixerType.Cider, ciderButton },
        { MixerType.Ginger, gingerButton },
        { MixerType.Lime, limeButton },
        { MixerType.Pimiento, pimientoButton }
    };

    }

    /// <summary>
    /// Sets a mixer in a specific slot for the player and changes the image displayed in slot.
    /// <param name="mixerType">The MixerType value of the button that was clicked by the player.</param>
    /// </summary>
    public void SelectMixer(MixerType mixerType)
    {
        mixerButtons.TryGetValue(mixerType, out Button button);
        button.gameObject.SetActive(false);
        currentSlot.sprite = button.GetComponent<SpriteRenderer>().sprite;
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
    /// <param name="baseType">The BaseType value of the button that was clicked by the player.</param>
    /// </summary>
    public void SelectBase(BaseType baseType)
    {
        baseButtons.TryGetValue(baseType, out Button button);
        button.gameObject.SetActive(false);
        currentSlot.sprite = button.GetComponent<SpriteRenderer>().sprite;
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
