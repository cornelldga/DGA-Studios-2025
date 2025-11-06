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
    Cider,
    Ginger,
    Lime,
    Pimiento,
}

/// <summary>
/// Container for all mixers that can be equipped, used to modify player stats and bases
/// </summary>
public class PlayerMixers : MonoBehaviour
{
    /// <summary>
    /// Mixers are associated to their corresponding <see cref="MixerType"/>
    /// </summary>
    [SerializeField] Mixer[] mixers;
    Dictionary<MixerType, Mixer> mixersDict = new Dictionary<MixerType, Mixer>();

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
    /// Returns the mixers of the associated <see cref="MixerType"/>
    /// </summary>
    /// <param name="type"></param>
    public Mixer GetMixer(MixerType mixer)
    {
        return mixersDict[mixer];
    }


}
