using UnityEngine;
/// <summary>
///  Increases player movement speed but decreases bullet accuracy
/// </summary>
[CreateAssetMenu(fileName = "Cider", menuName = "Mixers/Cider")]
public class Cider : Mixer
{
    [SerializeField] float ciderSpeedMultiplier;
    [SerializeField] float ciderAccuracyDecrease;
    public override void ApplyMixer(Base baseDrink)
    {
        baseDrink.accuracy += ciderAccuracyDecrease;
    }

    public override void ApplyMixer(Player player)
    {
        player.speed = player.baseSpeed * ciderSpeedMultiplier;
    }

    public override void RemoveMixer(Player player)
    {
        player.speed = player.baseSpeed;
    }
}
