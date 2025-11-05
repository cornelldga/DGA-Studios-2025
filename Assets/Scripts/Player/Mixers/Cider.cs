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
        baseDrink.speed *= ciderSpeedMultiplier;
        baseDrink.accuracy += ciderAccuracyDecrease;
    }

    public override void ApplyMixer(PlayerController player)
    {
        
    }

    public override void RemoveMixer(PlayerController player)
    {
        
    }
}
