using System;
using System.Collections.Generic;
using UnityEngine;

public class Smoker : MonoBehaviour
{
    //How fast should the smoker spin. (Can be modified later for different patterns)
    [SerializeField] float spinSpeed = 50;
    //How fast should smoke be shot out of smoker pipe.
    [SerializeField] float pelletSpeed = 3;
    [Tooltip("Fire rate of smoke")]
    [SerializeField] float smokeRate;
    //A counter on how long it has been since the last smoke was shot.
    float smokeTimer = 0f;
    [SerializeField] GameObject smokePelletPrefab;
    //Where the smoke should be released from.
    [SerializeField] Transform releasePoint;
    //A transform centered on the smoker to allow for 360 smoking.
    [SerializeField] Transform pivot;
    [SerializeField] TheMagician magician;


    [Header("Stages")]
    [SerializeField] Transform backStage;
    [SerializeField] Transform cardStage;
    [SerializeField] Transform doveStage;
    [SerializeField] Transform knifeStage;

    // Whether the stage has been hidden yet
    private bool hidStage;

    void Start()
    {
        hidStage = false;
    }

    // Update is called once per frame
    /// <summary>
    /// On update, the smoke releast point is pivoted to shot in 360 degrees over time. Shootig only happens if enough time has passed.
    /// </summary>
    void Update()
    {
        pivot.transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
        smokeTimer -= Time.deltaTime;
        if (magician.currentStage == Stage.Backstage)
        {
            if(smokeTimer <= 0)
            {
                ShootSmoke();
                smokeTimer = smokeRate;
                if (!hidStage)
                {
                    ObscureStage();
                }
            }
        }
        else { hidStage = false; }

    }
    /// <summary>
    /// Instantiates a pellet of smoke to be shot. It will rotate, and fire in the given direction.
    /// </summary>
    void ShootSmoke()
    {
        GameObject pellet = Instantiate(smokePelletPrefab, releasePoint.transform.position, Quaternion.identity);
        Rigidbody2D rb = pellet.GetComponent<Rigidbody2D>();
        pellet.transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f));
        Vector2 direction = new Vector2(releasePoint.position.x - transform.position.x, releasePoint.position.y - transform.position.y).normalized;
            if (rb != null)
            {
            rb.linearVelocity = direction * pelletSpeed * UnityEngine.Random.Range(0.7f, 1.0f);
            rb.angularVelocity = UnityEngine.Random.Range(-30f, 30f);
            }
    }

    void ObscureStage()
    {
        List<Stage> stages = new List<Stage>();
        stages.Add(Stage.Card);
        stages.Add(Stage.Knife);
        stages.Add(Stage.Dove);
        for (int i = 0; i < 2; i++)
        {
            Stage hideStage = stages[UnityEngine.Random.Range(0, stages.Count)];
            stages.Remove(hideStage);

            switch (hideStage)
            {
                case Stage.Card:
                    Instantiate(smokePelletPrefab, cardStage.position, Quaternion.identity);
                    break;
                case Stage.Dove:
                    Instantiate(smokePelletPrefab, doveStage.position, Quaternion.identity);
                    break;
                case Stage.Knife:
                    Instantiate(smokePelletPrefab, knifeStage.position, Quaternion.identity);
                    break;
            }
        }
        hidStage = true;
    }

}
