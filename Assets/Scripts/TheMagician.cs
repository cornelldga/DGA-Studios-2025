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
        //if timer is ever set to 0, magician changes stage
        if (timer == 0) {
            if (currentStage != Stage.Backstage)
            { currentStage = Stage.Backstage; }
            else { 
                int rStage = UnityEngine.Random.Range(1,4); 
                if(rStage == 1)currentStage=Stage.Knife;
                if (rStage == 2) currentStage = Stage.Card;
                if (rStage == 3) currentStage = Stage.Dove;
            }
        } 
        
        timer += Time.deltaTime;
        
    }
}
