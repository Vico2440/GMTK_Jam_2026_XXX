using UnityEngine;

public class PCAction : MonoBehaviour, IInteractableAction
{
    private bool isUsingPC = false;
    private PlayerController playerController;
    
    [SerializeField]
    private GameObject pc_ui;

    private void Awake()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        pc_ui.SetActive(false);
    }

    public void ExecuteAction()
    {
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
    /// Sécurité si le joueur est forcé de quitter la zone sans ré-interagir
    /// </summary>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isUsingPC)
        {
            isUsingPC = false;
            PresenceManager.Instance?.SetPlayerAtPC(false);

            if (playerController != null)
            {
                playerController.enabled = true;
            }
        }
    }
}