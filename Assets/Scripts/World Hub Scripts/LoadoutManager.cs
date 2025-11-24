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
    private Dictionary<BaseType, Button> baseButtons;
    private Dictionary<MixerType, Button> mixerButtons;
    private MixerType mixerType;
    private BaseType baseType;

    /// <summary>
    /// Initializes the baseButtons and mixerButtons before anything else.
    /// </summary>
    private void Awake()
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
    /// When Opened will make sure the slots are synced and any currently equipped bases and mixers dont have a button.
    /// </summary>
    private void OnEnable()
    {
        if (GameManager.Instance != null && GameManager.Instance.player != null)
        {
            SyncSlots();
            HideEquippedButtons();
        }
    }
    public void Close()
    {
        GameManager.Instance.ToggleLoadoutManager(false);
    }
    
    /// <summary>
    /// Sets a base in a specific slot for the player and changes the image displayed in slot.
    /// <param name="baseIndex">The BaseType value of the button that was clicked by the player.</param>
    /// </summary>
    public void SelectBase(int baseIndex)
    {
        baseType = (BaseType)baseIndex;
        baseButtons.TryGetValue(baseType, out Button button);
        
        int playerCurrentSlot = GameManager.Instance.player.GetCurrentBaseIndex();
        SpriteRenderer targetSlot = (playerCurrentSlot == 0) ? baseSlotOne : baseSlotTwo;
        
        targetSlot.sprite = button.GetComponent<Image>().sprite;
        
        BaseType replacedBase = GameManager.Instance.player.SwapBaseSlot(playerCurrentSlot, baseType);
        
        button.gameObject.SetActive(false);
        
        if (baseButtons.TryGetValue(replacedBase, out Button replacedButton))
        {
            replacedButton.gameObject.SetActive(true);
        }

        GameManager.Instance.player.RefreshUIIfNeeded(playerCurrentSlot, true);
        
        SyncSlots();
    }

    /// <summary>
    /// Sets a mixer in a specific slot for the player and changes the image displayed in slot.
    /// <param name="mixerIndex">The MixerType value of the button that was clicked by the player.</param>
    /// </summary>
    public void SelectMixer(int mixerIndex)
    {
        mixerType = (MixerType)mixerIndex;
        mixerButtons.TryGetValue(mixerType, out Button button);
        
        int playerCurrentSlot = GameManager.Instance.player.GetCurrentMixerIndex();
        SpriteRenderer targetSlot = (playerCurrentSlot == 0) ? mixerSlotOne : mixerSlotTwo;
        
        targetSlot.sprite = button.GetComponent<Image>().sprite;
        
        MixerType replacedMixer = GameManager.Instance.player.SwapMixerSlot(playerCurrentSlot, mixerType);
        
        button.gameObject.SetActive(false);
        
        if (mixerButtons.TryGetValue(replacedMixer, out Button replacedButton))
        {
            replacedButton.gameObject.SetActive(true);
        }

        GameManager.Instance.player.RefreshUIIfNeeded(playerCurrentSlot, false);
        
        SyncSlots();
    }

    /// <summary>
    /// Syncs the loadout slots with player's current inventory
    /// </summary>
    private void SyncSlots()
    {
        PlayerBases playerBasesComponent = GameManager.Instance.player.GetComponent<PlayerBases>();
        
        BaseType[] equippedBases = GameManager.Instance.player.GetEquippedBases();
        MixerType[] equippedMixers = GameManager.Instance.player.GetEquippedMixers();
        
        Base base1 = playerBasesComponent.GetBase(equippedBases[0]);
        Base base2 = playerBasesComponent.GetBase(equippedBases[1]);
        baseSlotOne.sprite = base1.getSprite();
        baseSlotTwo.sprite = base2.getSprite();
        
        if (mixerButtons.TryGetValue(equippedMixers[0], out Button mixer1Button))
        {
            mixerSlotOne.sprite = mixer1Button.GetComponent<Image>().sprite;
        }
        if (mixerButtons.TryGetValue(equippedMixers[1], out Button mixer2Button))
        {
            mixerSlotTwo.sprite = mixer2Button.GetComponent<Image>().sprite;
        }
    }

    /// <summary>
    /// Hides the buttons that are currently equipped mixers and bases.
    /// </summary>
    private void HideEquippedButtons()
    {
        BaseType[] playerBases = GameManager.Instance.player.GetEquippedBases();
        MixerType[] playerMixers = GameManager.Instance.player.GetEquippedMixers();
        
        foreach (BaseType baseType in playerBases)
        {
            if (baseButtons.TryGetValue(baseType, out Button button))
            {
                button.gameObject.SetActive(false);
            }
        }
        
        foreach (MixerType mixerType in playerMixers)
        {
            if (mixerButtons.TryGetValue(mixerType, out Button button))
            {
                button.gameObject.SetActive(false);
            }
        }
    }
}