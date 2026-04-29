using UnityEngine;

public class Bull : Pig
{
    private bool summoned;
    public void ChargeSpecificDirection(Vector3 randomDirection)
    {
        TransitionToCharging();
        this.chargeDirection = randomDirection.normalized;
    }

    public void setSummoned()
    {
        summoned = true;
    }
    
}
