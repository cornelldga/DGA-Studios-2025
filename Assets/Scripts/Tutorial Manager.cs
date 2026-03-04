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
        OnMove();
    }

    void OnStart()
    {
        if (TutorialTrigger.Equals("Start"))
        {
            OnInteract();
        }
    }

    void OnMove()
    {
        if (TutorialTrigger.Equals("Move") && !DialogueManager.Instance.OngoingDialogue() && !playerRB.linearVelocity.Equals(new Vector2(0, 0)))
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
