using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A Mixer modifies a base and/or player while equipped
/// </summary>
public abstract class Mixer : ScriptableObject
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
    /// <summary>
    /// Removes the mixer from the player
    /// </summary>
    /// <param name="player"></param>
    public abstract void RemoveMixer(PlayerController player);

}
