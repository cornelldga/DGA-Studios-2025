using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //How much faster the character should be moving right now as a multiplier.
    private float speedmultiplier;
    //Current speed
    private float speed;
    //Base speed
    private const int base_speed = 1;

    //How much health do we have at this point in time.
    private int health;
    //Base health
    private const int base_health = 100;
    //How much more damage should we recieve as a multiplier.
    private float damage_sens;
    //Basic sensitivity.
    private const int base_sensitivity = 1;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speed = base_speed;
        health = base_health;
        damage_sens = base_sensitivity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
}
