using UnityEngine;

public class TumbleweedOrbit : MonoBehaviour
{

    private Rigidbody2D rb;
    private CircleCollider2D thisCollider;
    private Animator animator;

    [SerializeField] private float orbitRadius = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        thisCollider = GetComponent<CircleCollider2D>();


    }



    private void Update()
    {
        


    }

    private void orbitAsh(float orbitRadius)
    {

    }


   /*<summary>
   Handles collisions for the orbiting tumbleweed
   Should not be interactible by the whip (?)
   should damage the player 
   should be killable by bullets
   if interacts w something on fire, should be set on fire. 

    </summary> */  
    private void OnCollisionEnter2D(Collision2D collision)
    {

    }
}
