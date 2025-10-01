using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// GameObject that reads triggers an interaction as long as the player is within range
/// </summary>

public class InteractionZone : MonoBehaviour
{
    [SerializeField] SpriteRenderer InteractionIndicator;

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

    void OnInteract()
    {
        if (canInteract)
        {
            Interact();
        }
    }

    void Interact()
    {
        Debug.Log("Interacting!");
    }


}
