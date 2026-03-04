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
        OnWhip();
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
        if (collision.CompareTag("Player") && TutorialTrigger.Equals("Move") && !DialogueManager.Instance.OngoingDialogue())
        {
            OnInteract();
        }

        if (collision.CompareTag("Base") && TutorialTrigger.Equals("Shoot") && !DialogueManager.Instance.OngoingDialogue())
        {
            OnInteract();
        }
    }

    void OnWhip()
    {
        if (TutorialTrigger.Equals("Whip") && !DialogueManager.Instance.OngoingDialogue() && Input.GetMouseButtonDown(1))
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
