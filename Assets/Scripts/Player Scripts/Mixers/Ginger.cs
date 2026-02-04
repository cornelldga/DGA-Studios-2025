using UnityEngine;
/// <summary>
/// Increases damage of projectiles deflected by whip but decreases damage done by normal player projectiles
/// </summary>
[CreateAssetMenu(fileName = "Ginger", menuName = "Mixers/Ginger")]

public class Ginger : Mixer
{
    [SerializeField] float gingerValueProjDamage;
    [SerializeField] float gingerWhipDamageMultiplier;
    [SerializeField] Color gingerColor;
    public override void ApplyMixer(Base baseDrink)
    {
        baseDrink.damage *= gingerValueProjDamage;
    }

    public override void ApplyMixer(Player player)
    {
        player.whip.damageMultiplier = gingerWhipDamageMultiplier;
        player.ChangeMixerEffect(gingerColor);
    }

    public override void RemoveMixer(Player player)
    {
        player.whip.damageMultiplier = player.whipBaseDamageMultiplier;
    }
}
