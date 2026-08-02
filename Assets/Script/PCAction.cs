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
            return; 
        }

        if (isUsingPC == false)
        {
            pc_ui.SetActive(true);
            
            int nombreAleatoire = Random.Range(1, 4);
            
            SoundManager.Instance.PlaySound("Visio" + nombreAleatoire);
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

        SoundManager.Instance.StopAllSounds();
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