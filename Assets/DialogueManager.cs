using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI Elements")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private CanvasGroup dialogueCanvasGroup; 
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float typingSpeed = 0.04f;

    [Header("Animation DOTween")]
    [SerializeField] private float fadeDuration = 0.25f;

    private Coroutine typingCoroutine;
    private bool isDialogueActive = false;
    private Tween activeTween;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (dialogueCanvasGroup != null)
        {
            dialogueCanvasGroup.alpha = 0f;
            dialoguePanel.transform.localScale = Vector3.one * 0.9f; 
        }
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(string text)
    {
        dialoguePanel.SetActive(true);
        isDialogueActive = true;

        if (activeTween != null && activeTween.IsActive()) activeTween.Kill();

        Sequence openSeq = DOTween.Sequence();
        openSeq.Join(dialogueCanvasGroup.DOFade(1f, fadeDuration));
        openSeq.Join(dialoguePanel.transform.DOScale(Vector3.one, fadeDuration).SetEase(Ease.OutBack));
        activeTween = openSeq;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeSentence(text));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void CloseDialogue()
    {
        if (!isDialogueActive) return;

        isDialogueActive = false;

        if (activeTween != null && activeTween.IsActive()) activeTween.Kill();

        Sequence closeSeq = DOTween.Sequence();
        closeSeq.Join(dialogueCanvasGroup.DOFade(0f, fadeDuration));
        closeSeq.Join(dialoguePanel.transform.DOScale(Vector3.one * 0.9f, fadeDuration).SetEase(Ease.InBack));
        closeSeq.OnComplete(() =>
        {
            dialoguePanel.SetActive(false);
            dialogueText.text = "";
        });

        activeTween = closeSeq;
    }

    public bool IsDialogueActive() => isDialogueActive;
}