using UnityEngine;

public class Bull : Pig
{
    public void ChargeSpecificDirection(Vector3 randomDirection)
    {
        TransitionToCharging();
        this.chargeDirection = randomDirection.normalized;
    }
    
}
