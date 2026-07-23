using UnityEngine;
using DG.Tweening;

public class Interactable : MonoBehaviour
{
    [Header("Dialogue")]
    [TextArea(3, 5)]
    [SerializeField] private string dialogueLine = "Insert Text Line";

    [Header("Animation DOTween")]
    [SerializeField] private float bounceScale = 1.15f;
    [SerializeField] private float bounceDuration = 0.2f;

    private bool isPlayerInZone = false;
    private Vector3 originalScale;
    private Tween activeTween;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void Interact()
    {
        if (!isPlayerInZone) return;

        AnimateBounce();

        if (DialogueManager.Instance != null)
        {
            if (DialogueManager.Instance.IsDialogueActive())
            {
                DialogueManager.Instance.CloseDialogue();
            }
            else
            {
                DialogueManager.Instance.StartDialogue(dialogueLine);
            }
        }
    }

    private void AnimateBounce()
    {
        if (activeTween != null && activeTween.IsActive())
        {
            activeTween.Kill();
            transform.localScale = originalScale;
        }

        activeTween = transform.DOPunchScale(originalScale * (bounceScale - 1f), bounceDuration, elasticity: 0.5f)
            .OnComplete(() => transform.localScale = originalScale);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInZone = true;
            collision.GetComponent<PlayerController>()?.SetCurrentInteractable(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInZone = false;
            collision.GetComponent<PlayerController>()?.SetCurrentInteractable(null);

            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.CloseDialogue();
            }
        }
    }
}