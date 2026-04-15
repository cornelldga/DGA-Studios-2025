using UnityEngine;

public class TumbleweedOrbit : Bullet
{

   
    [SerializeField] private GameObject ash;
    [SerializeField] private float orbitingSpeed = 1f;


    public override void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, duration);
    }



    private void Update()

    {
        Debug.Log("updating something");
        rb.transform.RotateAround(ash.transform.position, Vector3.up, orbitingSpeed * Time.deltaTime);

    }
}
