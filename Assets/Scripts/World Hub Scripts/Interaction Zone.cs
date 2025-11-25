using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// GameObject that reads triggers an interaction as long as the player is within range
/// </summary>

public class InteractionZone : MonoBehaviour
{
    [Tooltip("UI that indicates the player is in range to trigger the interaction")]
    [SerializeField] SpriteRenderer InteractionIndicator;
    [SerializeField] GameObject interactable;

    bool canInteract = false;

    private void Start()
    {
        InteractionIndicator.gameObject.SetActive(canInteract);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canInteract = true;
            InteractionIndicator.gameObject.SetActive(canInteract);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canInteract= false;
            InteractionIndicator.gameObject.SetActive(canInteract);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnInteract();
        }
    }

    void OnInteract()
    {
        if (canInteract)
        {
            Interact();
            canInteract = false;
            InteractionIndicator.gameObject.SetActive(false);
        }
    }

    void Interact()
    {
        interactable.GetComponent<IInteractable>().Interact();
    }


}
