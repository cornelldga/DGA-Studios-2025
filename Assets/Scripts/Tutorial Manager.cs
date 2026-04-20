using UnityEngine;

/// <summary>
/// GameObject that reads triggers that occur specifically in tutorial
/// </summary>

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private string TutorialTrigger;
    [SerializeField] GameObject interactable;

    [SerializeField] bool canInteract;

    private float hitTime;
    private float hitsInRow;

    private void Start()
    {
        hitTime = 0;
        hitsInRow = 1;
    }

    // Update is called once per frame
    void Update()
    {
        OnStart();
    }

    void OnStart()
    {
        if (TutorialTrigger.Equals("Start"))
        {
            OnInteract();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (TutorialTrigger)
        {
            case ("Move"):
                if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && !DialogueManager.Instance.OngoingDialogue())
                {
                    OnInteract();
                }
                break;
            case ("Shoot"):
                if (collision.gameObject.layer == LayerMask.NameToLayer("Base") && !DialogueManager.Instance.OngoingDialogue())
                {
                    OnInteract();
                }
                break;
            case ("Whip"):
                if (collision.gameObject.layer == LayerMask.NameToLayer("Base") && collision.gameObject.GetComponent<Bullet>() != null && collision.gameObject.GetComponent<Bullet>().Whipped() && !DialogueManager.Instance.OngoingDialogue())
                {
                    OnInteract();
                }
                break;
            case ("Whiskey"):
                if (collision.gameObject.layer == LayerMask.NameToLayer("Base") && collision.gameObject.GetComponent<Whiskey>() != null && !DialogueManager.Instance.OngoingDialogue())
                {
                    OnInteract();
                }
                break;
            case ("Cider"):
                if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && !DialogueManager.Instance.OngoingDialogue())
                {
                    OnInteract();
                }
                break;
            case ("Ginger"):
                if (collision.gameObject.layer == LayerMask.NameToLayer("Base") && !DialogueManager.Instance.OngoingDialogue())
                {
                    if (Time.time - hitTime < 0.261 && hitsInRow < 4)
                    {
                        hitsInRow++;
                    }
                    else if (Time.time - hitTime < 0.261 && hitsInRow >= 4)
                    {
                        OnInteract();
                    }
                    else if (Time.time - hitTime >= 0.261)
                    {
                        hitsInRow = 1;
                    }
                    hitTime = Time.time;
                }
                break;
            default:
                break;
        }
    }

    void OnInteract()
    {
        if (canInteract)
        {
            interactable.GetComponent<IInteractable>().Interact();
            canInteract = false;
        }
    }
}
