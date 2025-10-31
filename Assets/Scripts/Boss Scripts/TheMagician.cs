using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TheMagician : MonoBehaviour
{
    public BossManager BossManager;
    private enum Stage
    {
        Knife,Dove,Card,Backstage
    }
    [SerializeField] private GameObject stages;
    private GameObject knifeStage,doveStage,cardStage;
    private Stage currentStage;
    private float timer;

    // Time Magician spends attaacking
    public float attackDuration=0;
    // Time Magician spends off screen
    public float backstageDuration = 0;
    // Original stage locations
    private Vector3 firstStage,secondStage,thirdStage;
    //Current Boss Phase
    private int phase = 1;

    //Attack cool down
    [SerializeField] private float attackCooldown;
    private float attackTimer;
    // Player
    private GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        knifeStage = GameObject.Find("Knife");
        firstStage=knifeStage.transform.position;

        cardStage = GameObject.Find("Card");
        secondStage = cardStage.transform.position;

        doveStage = GameObject.Find("Dove");
        thirdStage = doveStage.transform.position;
        
        currentStage=Stage.Backstage;
        timer = 0;
        attackTimer = 0;
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // if timer is ever set to 0, magician changes stage This section of Code is responsible for switching the location of the Magician and updating
        // the currentStage Variable to be accurate
        if (timer == 0) {
            if (currentStage != Stage.Backstage)
            { 
                currentStage = Stage.Backstage;
                this.transform.position = new Vector3(10000,10000,10000);
            }
            else
            {
                Shuffle();
                int rStage = UnityEngine.Random.Range(1, 4);
                if (rStage == 1) { 
                    currentStage = Stage.Knife;
                    this.transform.position = knifeStage.transform.position;
                }
                if (rStage == 2)
                {
                    currentStage = Stage.Card;
                    this.transform.position = cardStage.transform.position;
                }
                if (rStage == 3)
                {
                    currentStage = Stage.Dove;
                    this.transform.position = doveStage.transform.position;
                }
            }
        }

        // Responsible for updating timer and setting it back to 0 when attack is done or when enough time is spent backstage
        timer += Time.deltaTime;
        attackTimer += Time.deltaTime;
        if (currentStage != Stage.Backstage)
        {
            if (timer > backstageDuration)
            {
                timer = 0;
            }
        }
        else
        {
            if (timer > attackDuration)
            {
                timer = 0;
            }
        }

        // Changes Boss Phase in response to health state
        if(BossManager.getHealth()/BossManager.getMaxHealth() <= .5 & phase ==1 )
        {
           phase = 2;
        }
        if (BossManager.getHealth() / BossManager.getMaxHealth() <= .25 & phase==2)
        {
            phase = 3;
        }

        //Put attacks here, check with timer variable if needed
        if (phase ==1)
        {
            if (attackTimer>attackCooldown)
            {
                attack();
                attackTimer = 0;
            }
            
        }
        else if (phase == 2)
        {
            if (attackTimer > attackCooldown*.7)
            {
                attack();
                attackTimer = 0;
            }
        }
        else if (phase == 3)
        {
            if (attackTimer > attackCooldown*.4)
            {
                attack();
                attackTimer = 0;
            }
        }
    }

    /// <summary>
    /// Sends out a bullet depending on what stage the magician is standing
    /// </summary>
    private void attack()
    {
        // "player" shold be the name of the player object if it's location is needed
        if (currentStage == Stage.Knife)
        { }
        else if (currentStage == Stage.Card)
        { }
        else if (currentStage == Stage.Dove)
        { }
    }




    /// <summary>
    /// Used to Determine if The Magician is OffStage
    /// </summary>
    /// <returns> true if The Magician is Backstage,
    /// false otherwise
    /// </returns>
    public bool IsOffStage()
    {
        if (currentStage == Stage.Backstage)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int GetPhase()
    {
        return phase;
    }

    /// <summary>
    /// Randomizes the types of Stages
    /// </summary>
    private void Shuffle()
    {
        List<GameObject> unassigned = new List<GameObject>();
        unassigned.Add(cardStage);
        unassigned.Add(doveStage);
        unassigned.Add(knifeStage);

        int rStage = UnityEngine.Random.Range(0, 3);
        unassigned[rStage].transform.position = firstStage;
        unassigned.RemoveAt(rStage);

        rStage = UnityEngine.Random.Range(0, 2);
        unassigned[rStage].transform.position = secondStage;
        unassigned.RemoveAt(rStage);

        unassigned[rStage].transform.position = thirdStage;
    }
}
