using UnityEditor;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private int startingHealth;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject healthBackDrop;
    private enum MovementType
    {
        Standing,MoveRight,MoveLeft,MoveUp,MoveDown,Circle,Line
    } 
    private MovementType movementType;
    private float timer;
    //Temp variables
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

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        healthBar.transform.localScale = new Vector3(health, 1, 1);
        
        if (health == 0)
        {
            Time.timeScale = 0;
        }
        
        //Imagined implementation: Select a location (Add Parameter?) and boss will pathfind, teleport, or jump to that location
        if (movementType != MovementType.Standing)
        {
            if (movementType == MovementType.Line)
            {
                if (timer > .01f )
               { transform.position = transform.position + new Vector3(.05f*right, 0, 0);
                    timer = 0;
                    moves++;
                }
                if (moves == 300/5)
                {
                    right = right *-1;
                    moves = 0;
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            health--;
        }
    }

    public int getHealth() { return health; }
    public void setHealth(int h) { health=h; }

}
