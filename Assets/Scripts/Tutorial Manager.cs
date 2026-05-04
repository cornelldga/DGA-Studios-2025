using UnityEngine;
using System.Collections;

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

    private void OnEnable()
    {
        if (TutorialTrigger == "Start")
        {
            canInteract = true;
            StartCoroutine(StartTutorialAfterSceneLoad());
        }
    }

    private IEnumerator StartTutorialAfterSceneLoad()
    {
        yield return new WaitForSeconds(0.2f);
        OnInteract();
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
            case ("Cider"):
                if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && !GameManager.Instance.GetDialogueManager.OngoingDialogue())
                {
                    OnInteract();
                }
                break;
            case ("Ginger"):
                if (collision.gameObject.layer == LayerMask.NameToLayer("Base") && !GameManager.Instance.GetDialogueManager.OngoingDialogue())
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
        if (!canInteract || interactable == null) return;

        IInteractable target = interactable.GetComponent<IInteractable>();

        if (target != null)
        {
            GameManager.Instance.GetDialogueManager.SetNameTextVisible(false);

            target.Interact();
            canInteract = false;
            StartCoroutine(ResetNameTextAfterDialogue());
        }
    }

    /// <summary>
    /// Fades in before starting mid cutscene
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayMidCutsceneAfterFade()
    {
        GameManager.Instance.FreezePlayer(true);
        CutsceneManager.Instance.PlayFadeStart();
        yield return new WaitForSeconds(1f);
        CutsceneManager.Instance.PlayMidCutscene();
    }

    /// <summary>
    /// Waits for dialogue to end before turning name text back on.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResetNameTextAfterDialogue()
    {
        DialogueManager dialogueManager = GameManager.Instance.GetDialogueManager;

        yield return new WaitUntil(() => !dialogueManager.gameObject.activeSelf);

        dialogueManager.SetNameTextVisible(true);

        if (TutorialTrigger == "Ginger")
        {
            StartCoroutine(PlayMidCutsceneAfterFade());
        }
    }
}