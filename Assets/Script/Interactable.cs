using UnityEngine;
using DG.Tweening;

public class Interactable : MonoBehaviour
{
    [Header("Indicateur d'Interaction (UI)")]
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private float animDuration = 0.25f;
    [SerializeField] private Vector3 promptTargetScale = Vector3.one; // (1,1,1) par défaut

    [Header("Animation Bounce au Clic")]
    [SerializeField] private float bounceScale = 1.15f;
    [SerializeField] private float bounceDuration = 0.2f;

    private bool isPlayerInZone = false;
    private bool isInteracting = false;
    private Vector3 originalObjectScale;
    private Vector3 originalPromptScale;
    private Tween activeObjectTween;
    private Tween activePromptTween;

    private void Awake()
    {
        originalObjectScale = transform.localScale; 

        if (interactionPrompt != null)
        {
            originalPromptScale = interactionPrompt.transform.localScale;
            interactionPrompt.transform.localScale = Vector3.zero;
            interactionPrompt.SetActive(false);
        }
    }

    public void Interact()
    {
        if (!isPlayerInZone) return;

        AnimateObjectBounce();

        var action = GetComponent<IInteractableAction>();

        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive())
        {
            DialogueManager.Instance.CloseDialogue();
            isInteracting = false;
            ShowPrompt();
        }
        else
        {
            isInteracting = true;
            HidePrompt();
            action?.ExecuteAction();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInZone = true;
            collision.GetComponent<PlayerController>()?.SetCurrentInteractable(this);

            if (!isInteracting)
            {
                ShowPrompt();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInZone = false;
            isInteracting = false;
            collision.GetComponent<PlayerController>()?.SetCurrentInteractable(null);

            HidePrompt();

            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.CloseDialogue();
            }
        }
    }


    private void ShowPrompt()
    {
        if (interactionPrompt == null) return;


        interactionPrompt.SetActive(true);

        if (activePromptTween != null && activePromptTween.IsActive())
            activePromptTween.Kill();

        activePromptTween = interactionPrompt.transform
            .DOScale(promptTargetScale, animDuration)
            .SetEase(Ease.OutBack);
    }

    private void HidePrompt()
    {
        if (interactionPrompt == null) return;


        if (activePromptTween != null && activePromptTween.IsActive())
            activePromptTween.Kill();

        activePromptTween = interactionPrompt.transform
            .DOScale(Vector3.zero, animDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() => 
            {
                if (!isPlayerInZone || isInteracting) 
                {
                    interactionPrompt.SetActive(false);
                }
            });
    }

    

    private void AnimateObjectBounce()
    {
        if (activeObjectTween != null && activeObjectTween.IsActive())
        {
            activeObjectTween.Kill();
            transform.localScale = originalObjectScale;
        }

        activeObjectTween = transform.DOPunchScale(originalObjectScale * (bounceScale - 1f), bounceDuration, elasticity: 0.5f)
            .OnComplete(() => transform.localScale = originalObjectScale);
    }
}