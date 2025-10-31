using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
/// <summary>
/// The type of mixer
/// </summary>

public enum MixerType
{
    Lime = 0,
    Pimiento,
    Ginger,
    Cider,
}

/// <summary>
/// Container for all mixers that can be equipped, used to modify player stats and bases
/// </summary>
public class PlayerMixers : MonoBehaviour
{
    [SerializeField] Mixer[] mixers;
    Dictionary<MixerType, Mixer> mixersDict = new Dictionary<MixerType, Mixer>();

    Mixer selectedMixer;

    private void Start()
    {
        if(mixers.Length != sizeof(MixerType)){
            throw new InvalidImplementationException("Mixer array must match the size of MixerType and in order of mixer type");
        }
        for(int i = 0; i < mixers.Length; i++)
        {
            mixersDict[(MixerType)i] = mixers[i];
        }
    }

    /// <summary>
    /// Sets the selected mixer
    /// </summary>
    /// <param name="type"></param>
    public void SelectMixer(MixerType type)
    {
        selectedMixer = mixersDict[type];
    }
    /// <summary>
    /// Gets the selected mixer
    /// </summary>
    /// <returns>The selected mixer</returns>
    public Mixer GetMixer()
    {
        return selectedMixer;
    }


}

/// <summary>
/// A Mixer modifies a base and/or player while equipped
/// </summary>

public abstract class Mixer : MonoBehaviour
{
    /// <summary>
    /// Applies the mixer to the given base
    /// </summary>
    /// <param name="baseDrink"></param>
    public abstract void ApplyMixer(Base baseDrink);

    /// <summary>
    /// Applies the mixer to the player
    /// </summary>
    /// <param name="player"></param>
    public abstract void ApplyMixer(PlayerController player);
}
