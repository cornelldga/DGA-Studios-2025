using UnityEngine;

/// <summary>
/// GameObject that reads triggers that occur specifically in tutorial
/// </summary>

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private string TutorialTrigger;
    [SerializeField] GameObject interactable;

    [SerializeField] bool canInteract;

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
                if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && !GameManager.Instance.GetDialogueManager.OngoingDialogue())
                {
                    OnInteract();
                }
                break;
            case ("Shoot"):
                if (collision.gameObject.layer == LayerMask.NameToLayer("Base") && !GameManager.Instance.GetDialogueManager.OngoingDialogue())
                {
                    OnInteract();
                }
                break;
            case ("Whip"):
                if (collision.gameObject.layer == LayerMask.NameToLayer("Base") && collision.gameObject.GetComponent<Bullet>() != null && collision.gameObject.GetComponent<Bullet>().Whipped() && !GameManager.Instance.GetDialogueManager.OngoingDialogue())
                {
                    OnInteract();
                }
                break;
            case ("Whiskey"):
                if (collision.gameObject.layer == LayerMask.NameToLayer("Base") && collision.gameObject.GetComponent<Whiskey>() != null && !GameManager.Instance.GetDialogueManager.OngoingDialogue())
                {
                    OnInteract();
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
