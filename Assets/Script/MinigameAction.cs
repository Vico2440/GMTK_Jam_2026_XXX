using UnityEngine;

public class MinigameAction : MonoBehaviour, IInteractableAction
{
    [SerializeField] private string minigameID = "BoxWifi"; 
    
    [Header("Visuel Alerte (Optionnel)")]
    [Tooltip("Ex: Un petit icône rouge au dessus de l'objet")]
    [SerializeField] private GameObject alertIcon; 

    private void Update()
    {
        if (alertIcon != null && CrisisManager.Instance != null)
        {
            bool isBroken = CrisisManager.Instance.IsCrisisActive(minigameID);
            
            if (alertIcon.activeSelf != isBroken)
            {
                alertIcon.SetActive(isBroken);
            }
        }
    }

    public void ExecuteAction()
    {
        if (CrisisManager.Instance.IsCrisisActive(minigameID))
        {
            MinigameManager.Instance?.StartMinigame(minigameID);
        }
        else
        {
            Debug.Log($"L'objet {minigameID} fonctionne parfaitement. Inutile d'y toucher !");
        }
    }
}