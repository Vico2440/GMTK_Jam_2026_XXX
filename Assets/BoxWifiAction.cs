using UnityEngine;

public class BoxWifiAction : MonoBehaviour, IInteractableAction
{
    [Header("Canvas du Mini-Jeu")]
    [SerializeField] private GameObject boxWifiCanvasUI;

    public void ExecuteAction()
    {
        if (boxWifiCanvasUI != null)
        {
            boxWifiCanvasUI.SetActive(true);
        }
    }
}