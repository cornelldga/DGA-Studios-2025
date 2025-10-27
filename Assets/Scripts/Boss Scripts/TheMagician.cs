using System;
using Unity.VisualScripting;
using UnityEngine;

public class TheMagician : MonoBehaviour
{
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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        knifeStage = GameObject.Find("Knife");
        cardStage = GameObject.Find("Card");
        doveStage = GameObject.Find("Dove");
        currentStage=Stage.Backstage;
        timer = 0;
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
                Shuffle();
            }
        }
       //______________________________________________

        // Responsible for updating timer and setting it back to 0 
        timer += Time.deltaTime;
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

    }

    /// <summary>
    /// Used to Determine if The Magician is OffStage
    /// </summary>
    /// <returns> true if The Magician is Backstage,
    /// false otherwise
    /// </returns>
    public bool IsOffStage()
    { 
    if(currentStage == Stage.Backstage) return true;
    else return false;
    }

    /// <summary>
    /// Randomizes the types of Stages
    /// </summary>
    private void Shuffle()
    { 
    
    }
}
