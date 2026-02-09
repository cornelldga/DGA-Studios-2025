using UnityEngine;
/// <summary>
/// Increases player damage done but increases player damage taken
/// </summary>
[CreateAssetMenu(fileName = "Pimiento", menuName = "Mixers/Pimiento")]
public class Pimiento : Mixer
{
    [SerializeField] float pimientoValue;
    [SerializeField] Color pimientoColor;
    public override void ApplyMixer(Base baseDrink)
    {
        baseDrink.damage *= pimientoValue;
    }

    public override void ApplyMixer(Player player)
    {
        player.damageTakenMultiplier *= pimientoValue;
        player.ChangeMixerEffect(pimientoColor);
    }

    public override void RemoveMixer(Player player)
    {
        player.damageTakenMultiplier /= pimientoValue;
    }
}
