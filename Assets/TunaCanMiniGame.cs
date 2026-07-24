using UnityEngine;
using UnityEngine.EventSystems; 
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class TunaCanMiniGame : MonoBehaviour, IDragHandler
{
    [Header("UI Elements")]
    [SerializeField] private RectTransform lidRect; 
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Réglages")]
    [Tooltip("Sensibilité du glissement de la souris")]
    [SerializeField] private float dragSensitivity = 1.5f; 
    
    [Tooltip("Distance totale à parcourir avec la souris pour faire 1 tour (en pixels)")]
    [SerializeField] private float requiredDragDistance = 800f; 
    
    [Header("Configuration")]
    [Tooltip("Le MÊME nom que dans ton MinigameManager (ex: TunaCan)")]
    [SerializeField] private string minigameID = "TunaCan";

    [SerializeField] private Sprite openTuna;
    [SerializeField] private Sprite closedTuna;
    [SerializeField] private Image TunaImage;

    private float currentDragProgress = 0f;
    private bool isCompleted = false;
    private Vector2 originalLidPosition;
    
    private void Awake()
    {
        TunaImage.sprite = closedTuna;
        
        if (lidRect != null)
        {
            originalLidPosition = lidRect.anchoredPosition;
        }
    }

    private void OnEnable()
    {
        currentDragProgress = 0f;
        isCompleted = false;
    
        if (lidRect != null)
        {
            lidRect.localEulerAngles = Vector3.zero; 
        
            lidRect.anchoredPosition = originalLidPosition; 
        }

        if (statusText != null)
        {
            statusText.text = "GLISSE VERS LA GAUCHE POUR OUVRIR !";
            statusText.color = Color.white;
        }
    }

    /// <summary>
    /// Cette fonction est appelée automatiquement par Unity à CHAQUE FRAME 
    /// où le joueur maintient le clic enfoncé et bouge la souris.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (isCompleted) return;

        if (eventData.delta.x < 0)
        {
            currentDragProgress += Mathf.Abs(eventData.delta.x) * dragSensitivity;

            UpdateLidRotation();

            if (currentDragProgress >= requiredDragDistance)
            {
                CompleteMiniGame();
            }
        }
    }

    private void UpdateLidRotation()
    {
        if (lidRect == null) return;

        float progress = Mathf.Clamp01(currentDragProgress / requiredDragDistance);

        float currentAngle = progress * 360f;

        lidRect.localEulerAngles = new Vector3(0, 0, currentAngle);
    }

    private void CompleteMiniGame()
    {
        isCompleted = true;

        if (statusText != null)
        {
            TunaImage.sprite = openTuna;
            
            statusText.text = "BOÎTE OUVERTE ! LE CHAT EST CONTENT !";
            statusText.color = Color.green;
        }

        CrisisManager.Instance?.ResolveCrisis(minigameID);

        lidRect.DOAnchorPosY(lidRect.anchoredPosition.y + 200f, 0.5f).SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    MinigameManager.Instance?.CloseMinigame(minigameID);
                });
            });
    }
}
