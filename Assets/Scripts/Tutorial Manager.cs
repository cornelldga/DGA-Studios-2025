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
        OnShoot();
        OnWhip();
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

    void OnShoot()
    {
        if (TutorialTrigger.Equals("Shoot") && !DialogueManager.Instance.OngoingDialogue() && Input.GetMouseButtonDown(0))
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
