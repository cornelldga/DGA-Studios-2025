using UnityEngine;
/// <summary>
/// Increases player fire rate but slows player movement speed
/// </summary>
[CreateAssetMenu(fileName = "Lime", menuName = "Mixers/Lime")]
public class Lime : Mixer
{
    [SerializeField] float limeJuiceValue;
    public override void ApplyMixer(Base baseDrink)
    {
        baseDrink.cooldown *= limeJuiceValue;
    }
    public override void ApplyMixer(Player player)
    {
        player.speed = player.baseSpeed * limeJuiceValue;
    }

    public override void RemoveMixer(Player player)
    {
        player.speed = player.baseSpeed;
    }
}
