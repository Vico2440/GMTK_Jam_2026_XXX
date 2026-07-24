using UnityEngine;

public class MinigameAction : MonoBehaviour, IInteractableAction
{
    [SerializeField] private string minigameID = "CutWire"; 

    public void ExecuteAction()
    {
        //MinigameManager.Instance?.StartMinigame(minigameID);
    }
}