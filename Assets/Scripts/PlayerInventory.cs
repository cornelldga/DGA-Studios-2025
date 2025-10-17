using System.Data.Common;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    //What mixer types do we have
    public enum MixerType
    {
        Lime,
        Pimiento,
        None,

    }
    //What base types do we have.
    public enum BaseType
    {
        Beer,
        Gin,
        Whiskey,
        Wine,
        None,
    }

    [Header("Mixer Inventory")]
    [SerializeField] private MixerType mixerSlot1 = MixerType.None;
    [SerializeField] private MixerType mixerSlot2 = MixerType.None;

    [Header("Base Inventory")]
    [SerializeField] private BaseType baseSlot1 = BaseType.None;
    [SerializeField] private BaseType baseSlot2 = BaseType.None;

    [Header("Currently Equipped")]
    [SerializeField] private MixerType equippedMixer = MixerType.None;
    [SerializeField] private BaseType equippedBase = BaseType.None;

    [Header("Selected Slots")]
    [SerializeField] private int selectedBaseSlot = 1; // 1 or 2
    [SerializeField] private int selectedMixerSlot = 1; // 1 or 2

    //Could use prefabs of each mixer/base here, but figured seperate enums could work as well.

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateEquippedItems();
    }

    // Update is called once per frame
    void Update()
    {

    }

     public void SelectBaseSlot(int slotIndex)
    {
        if (slotIndex == 1 || slotIndex == 2)
        {
            selectedBaseSlot = slotIndex;
            UpdateEquippedBase();
        }
    }

    public void SelectMixerSlot(int slotIndex)
    {
        if (slotIndex == 1 || slotIndex == 2)
        {
            selectedMixerSlot = slotIndex;
            UpdateEquippedMixer();
        }
    }

    private void UpdateEquippedBase()
    {
        equippedBase = GetBaseInSlot(selectedBaseSlot);
    }

    private void UpdateEquippedMixer()
    {
        equippedMixer = GetMixerInSlot(selectedMixerSlot);
    }

    private void UpdateEquippedItems()
    {
        UpdateEquippedBase();
        UpdateEquippedMixer();
    }

    public bool AddMixer(MixerType mixerType)
    {
        if (mixerSlot1 == MixerType.None)
        {
            mixerSlot1 = mixerType;
            return true;
        }
        else if (mixerSlot2 == MixerType.None)
        {
            mixerSlot2 = mixerType;
            return true;
        }
        else //Inventory full
        {
            return false;
        }
    }
    public void SetMixerInSlot(int slotIndex, MixerType mixerType)
    {
        if (slotIndex == 1)
        {
            mixerSlot1 = mixerType;
        }
        else if (slotIndex == 2)
        {
            mixerSlot2 = mixerType;
        }
    }
    public MixerType GetMixerInSlot(int slotIndex)
    {
        if (slotIndex == 1)
        {
            return mixerSlot1;
        }
        else if (slotIndex == 2)
        {
            return mixerSlot2;
        }
        else
        {
            return MixerType.None;
        }
    }
    public bool EquipMixer(int slotIndex)
    {
        MixerType mixer = GetMixerInSlot(slotIndex);
        if (mixer != MixerType.None)
        {
            equippedMixer = mixer;
            return true;
        }
        return false;
    }
     public MixerType GetEquippedMixer()
    {
        return equippedMixer;
    }

    public bool AddBase(BaseType baseType)
    {
        if (baseSlot1 == BaseType.None)
        {
            baseSlot1 = baseType;
            return true;
        }
        else if (baseSlot2 == BaseType.None)
        {
            baseSlot2 = baseType;
            return true;
        }
        else //Inventory full
        {
            return false;
        }
    }
    public void SetBaseInSlot(int slotIndex, BaseType baseType)
    {
        if (slotIndex == 1)
        {
            baseSlot1 = baseType;
        }
        else if (slotIndex == 2)
        {
            baseSlot2 = baseType;
        }
    }
    public BaseType GetBaseInSlot(int slotIndex)
    {
        if (slotIndex == 1)
        {
            return baseSlot1;
        }
        else if (slotIndex == 2)
        {
            return baseSlot2;
        }
        else
        {
            return BaseType.None;
        };
    }
    public bool EquipBase(int slotIndex)
    {
        BaseType base_ = GetBaseInSlot(slotIndex); //base is a keyword.
        if (base_ != BaseType.None)
        {
            equippedBase = base_;
            return true;
        }
        return false;
    }
    public BaseType GetEquippedBase()
    {
        return equippedBase;
    }


    public void ClearInventory()
    {
        mixerSlot1 = MixerType.None;
        mixerSlot2 = MixerType.None;
        baseSlot1 = BaseType.None;
        baseSlot2 = BaseType.None;
        equippedMixer = MixerType.None;
        equippedBase = BaseType.None;
    }
}
