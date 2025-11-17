using UnityEngine;

public class Pig : MonoBehaviour, IDamageable
{
    public enum State
    {
        Charging, Targeting, Patrolling
    }

    public void TakeDamage(float damage)
    {
        //TODO: damage logic here
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
