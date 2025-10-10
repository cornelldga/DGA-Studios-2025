using UnityEditor;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    [Header("Health Parameters")]
    [SerializeField] private int health;
    [SerializeField] private int startingHealth;
    
    [Header("Temporary Health Bar Assets")]
    // Healthbar for testing purposes
    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject healthBackDrop;
    
    // Types of movement the boss can do (Temporary)
    private enum MovementType
    {
        Standing,MoveRight,MoveLeft,MoveUp,MoveDown,Circle,Line
    } 

    //Current Movement Pattern
    private MovementType movementType;
    private float timer;

    //Temp variables for moving boss
    private int moves;
    private int right;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = startingHealth;
        healthBar.SetActive(true);
        healthBackDrop.SetActive(true);
        healthBackDrop.transform.localScale = new Vector3(startingHealth,1,1);
        healthBar.transform.localScale = new Vector3(startingHealth, 1, 1);
        movementType=MovementType.Line;
        timer = 0;
        moves = 0;
        right = 1;
    }

    /// <summary>
    /// Update is called once per frame, the debug health bar and timer variable
    /// </summary>
    void Update()
    {
        timer += Time.deltaTime;
        healthBar.transform.localScale = new Vector3(health, 1, 1);
        
        if (health == 0)
        {
            Time.timeScale = 0;
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    /// <summary>
    /// Responsible for Boss Movement. Currently moves back and forth, but should be different for every boss.
    /// </summary>
    private void Move()
    {
        //Imagined future implementation: Select a location (Add Parameter?) and boss will pathfind, teleport, or jump to that location
        if (movementType != MovementType.Standing)
        {
            if (movementType == MovementType.Line)
            {
                if (timer > .01f)
                {
                    transform.position = transform.position + new Vector3(.05f * right, 0, 0);
                    timer = 0;
                    moves++;
                }
                if (moves == 300 / 5)
                {
                    right = right * -1;
                    moves = 0;
                }
            }
        }
    }

    /// <summary>
    /// Responisble for decreasing boss health when making contact with a player bullet. Currently Destroys bullets that harm it
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            health--;
            Destroy(collision.gameObject);
        }
    }


    public int getHealth() { return health; }
    public void setHealth(int h) { health=h; }

}
