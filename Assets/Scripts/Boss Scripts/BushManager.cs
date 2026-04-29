using UnityEngine;

public class BushManager : MonoBehaviour
{

    private GameObject ash;



    private SpriteRenderer sr;
    private SpriteRenderer ashSR;
    private SpriteRenderer plyrSR;

    //[Header("Numbers for bush layer rendering")]
    private float dukeFootOffset = .3f;
    private float ashFootOffset = .7f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ash = GameObject.Find("Ash");
        sr = GetComponent<SpriteRenderer>();
        ashSR = ash.GetComponent<SpriteRenderer>();
        plyrSR = GameManager.Instance.player.GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        bool frontOfDuke = false;
        bool frontOfAsh = false;
        bool FOD = false;
        bool FOA = false;
        GameObject[] bushes = GameObject.FindGameObjectsWithTag("Bush");

        foreach (GameObject bushesObj in bushes)
        {
            sr = bushesObj.GetComponent<SpriteRenderer>();

            if ((ash.transform.position.y - ashFootOffset) >= bushesObj.transform.position.y)
            {
                frontOfAsh = true;
            }
            else
            {
                frontOfAsh = false;
            }

            if ((GameManager.Instance.player.transform.position.y - dukeFootOffset) >= bushesObj.transform.position.y)
            {
                frontOfDuke = true;
            }
            else
            {
                frontOfDuke = false;
            }

            if (frontOfDuke)
            {
                if (frontOfAsh)
                {
                    sr.sortingOrder = 5;
                }
                else
                {
                    sr.sortingOrder = 3;
                   // ashSR.sortingOrder = 4;
                   // plyrSR.sortingOrder = 2;
                   FOD = true;
                }

            }
            else
            {
                if (frontOfAsh)
                {
                    sr.sortingOrder = 3;
                    //ashSR.sortingOrder = 2;
                    //plyrSR.sortingOrder = 4;
                    FOA = true;
                }
                else
                {

                    sr.sortingOrder = 1;
                }
            }
        }

        if (FOA && !FOD)
        {
            ashSR.sortingOrder = 2;
            plyrSR.sortingOrder = 4;
        }

        if (FOD && !FOA)
        {
            ashSR.sortingOrder = 4;
            plyrSR.sortingOrder = 2;
        }

    }
}
