using UnityEngine;
using System.Collections.Generic;

public class PCAction : MonoBehaviour, IInteractableAction
{
    private bool isUsingPC = false;
    private PlayerController playerController;
    
    [SerializeField]
    private GameObject pc_ui;

    private void Awake()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        if (pc_ui != null) pc_ui.SetActive(false);
    }

    private void Update()
    {
        if (isUsingPC && HasAnyActiveCrisis())
        {
            ForceClosePC();
        }
    }

    public void ExecuteAction()
    {
        if (HasAnyActiveCrisis())
        {
            Debug.Log(">>> Impossible d'utiliser le PC : Il y a des pannes à régler !");
            return; 
        }

        if (isUsingPC == false)
        {
            pc_ui.SetActive(true);
        }
        else
        {
            pc_ui.SetActive(false);
        }
        
        isUsingPC = !isUsingPC;
        
        PresenceManager.Instance?.SetPlayerAtPC(isUsingPC);

        if (playerController != null)
        {
            playerController.enabled = !isUsingPC;

            if (playerController.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                rb.linearVelocity = Vector2.zero;
            }
        }

        Debug.Log(isUsingPC ? ">>> Connecté au PC (Présence ++)" : "<<< Debout du PC (Présence --)");
    }

    /// <summary>
    /// Force la fermeture du PC et redonne le contrôle au joueur
    /// </summary>
    private void ForceClosePC()
    {
        isUsingPC = false;
        if (pc_ui != null) pc_ui.SetActive(false);
        
        PresenceManager.Instance?.SetPlayerAtPC(false);

        if (playerController != null)
        {
            playerController.enabled = true;
        }

        Debug.Log(">>> ALARME ! Éjection forcée du PC à cause d'une crise !");
    }

    /// <summary>
    /// Vérifie si au moins un mini-jeu / crise est en cours
    /// </summary>
    private bool HasAnyActiveCrisis()
    {
        if (CrisisManager.Instance == null) return false;

        return CrisisManager.Instance.IsAnyCrisisActive();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isUsingPC)
        {
            ForceClosePC();
        }
    }
}