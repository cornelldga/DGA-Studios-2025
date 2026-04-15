using UnityEngine;

public class TumbleweedOrbit : Bullet
{

   
  
    [SerializeField] private float orbitingSpeed = 360f;
    private GameObject ash;


    public override void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ash = GameObject.Find("Ash");

        Destroy(gameObject, duration);
    }



    private void FixedUpdate()

    {
        Debug.Log(ash.transform.position);
        transform.RotateAround(ash.transform.position, Vector3.back, orbitingSpeed);

    }
}
