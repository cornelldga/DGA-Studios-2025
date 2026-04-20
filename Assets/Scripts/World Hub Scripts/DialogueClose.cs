using UnityEngine;

public class DialogueClose : MonoBehaviour
{
    public void CallDialogueClose()
    {
        GameManager.Instance.GetDialogueManager.AnimationCloseDialogue();
    }
}
