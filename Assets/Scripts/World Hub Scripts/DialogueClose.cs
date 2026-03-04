using UnityEngine;

public class DialogueClose : MonoBehaviour
{
    public void CallDialogueClose()
    {
        DialogueManager.Instance.AnimationCloseDialogue();
    }
}
