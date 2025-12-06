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

    [SerializeField] bool canInteract;

    private void Start()
    {
        SetCanInteract(canInteract);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SetCanInteract(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SetCanInteract(false);
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
            interactable.GetComponent<IInteractable>().Interact();
            canInteract = false;
            InteractionIndicator.gameObject.SetActive(false);
        }
    }

    public void SetCanInteract(bool canInteract)
    {
        this.canInteract = canInteract;
        InteractionIndicator.gameObject.SetActive(canInteract);
    }


}
