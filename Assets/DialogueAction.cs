using UnityEngine;

public class DialogueAction : MonoBehaviour, IInteractableAction
{
    [TextArea(3, 5)]
    [SerializeField] private string dialogueLine = "Test Test test";

    public void ExecuteAction()
    {
        if (DialogueManager.Instance != null)
        {
            if (DialogueManager.Instance.IsDialogueActive())
                DialogueManager.Instance.CloseDialogue();
            else
                DialogueManager.Instance.StartDialogue(dialogueLine);
        }
    }
}