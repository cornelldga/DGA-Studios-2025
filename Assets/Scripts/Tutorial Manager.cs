using UnityEngine;

/// <summary>
/// GameObject that reads triggers that occur specifically in tutorial
/// </summary>

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private string TutorialTrigger;
    [SerializeField] GameObject interactable;

    [SerializeField] bool canInteract;

    [SerializeField] Rigidbody2D playerRB;

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
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && TutorialTrigger.Equals("Move") && !DialogueManager.Instance.OngoingDialogue())
        {
            OnInteract();
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Base") && TutorialTrigger.Equals("Shoot") && !DialogueManager.Instance.OngoingDialogue())
        {
            OnInteract();
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Base") && TutorialTrigger.Equals("Whip") && canInteract && collision.gameObject.GetComponent<Bullet>().Whipped() && !DialogueManager.Instance.OngoingDialogue())
        {
            OnInteract();
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
