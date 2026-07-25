using UnityEngine;

public class MinigameAction : MonoBehaviour, IInteractableAction
{
    [SerializeField] private string minigameID = "BoxWifi"; 
    
    [Header("Visuel Alerte (Prefab)")]
    [Tooltip("Le Prefab de ton icône d'alerte")]
    [SerializeField] private GameObject alertPrefab; 
    
    [Tooltip("L'endroit exact où l'icône doit apparaître (Objet vide)")]
    [SerializeField] private Transform alertSpawnPoint;

    private GameObject spawnedAlert; 

    private void Update()
    {
        if (alertPrefab != null && CrisisManager.Instance != null)
        {
            bool isBroken = CrisisManager.Instance.IsCrisisActive(minigameID);
            
            if (isBroken && spawnedAlert == null)
            {
                Transform spawnParent = alertSpawnPoint != null ? alertSpawnPoint : transform;
                
                spawnedAlert = Instantiate(alertPrefab, spawnParent.position, Quaternion.identity, spawnParent);
            }
            else if (!isBroken && spawnedAlert != null)
            {
                Destroy(spawnedAlert);
                spawnedAlert = null; 
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