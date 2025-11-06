using UnityEngine;
/// <summary>
/// Increases damage of projectiles deflected by whip but decreases damage done by normal player projectiles
/// </summary>
[CreateAssetMenu(fileName = "Ginger", menuName = "Mixers/Ginger")]

public class Ginger : Mixer
{
    [SerializeField] float gingerValueProjDamage;
    [SerializeField] float gingerWhipDamageMultiplier;
    public override void ApplyMixer(Base baseDrink)
    {
        baseDrink.damage *= gingerValueProjDamage;
    }

    public override void ApplyMixer(PlayerController player)
    {
        player.whip.damageMultiplier = gingerWhipDamageMultiplier;
    }

    public override void RemoveMixer(PlayerController player)
    {
        player.whip.damageMultiplier = player.whipBaseDamageMultiplier;
    }
}
