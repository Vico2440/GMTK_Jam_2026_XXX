using UnityEngine;
using UnityEngine.EventSystems; // Indispensable pour OnDrag et OnEndDrag
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class TunaCanMiniGame : MonoBehaviour, IDragHandler, IEndDragHandler 
{
    private enum Phase { Opening, Pouring, Done }
    private Phase currentPhase = Phase.Opening;

    [Header("Configuration")]
    [SerializeField] private string minigameID = "TunaCan";

    [Header("UI - Phase 1 (Ouvrir)")]
    [SerializeField] private RectTransform canPhase1Rect; // La boîte normale
    [SerializeField] private RectTransform lidRect;       // Le couvercle
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("UI - Phase 2 (Servir)")]
    [SerializeField] private RectTransform canPhase2Rect; 
    [SerializeField] private RectTransform tunaFoodRect;  

    [Header("Réglages")]
    [SerializeField] private float dragSensitivity = 1.0f;
    [SerializeField] private float requiredDragDistance = 2500f; 
    [SerializeField] private float pullDownThreshold = 150f;     

    private float currentDragProgress = 0f;
    
    private Vector2 originalLidPosition;
    private Vector2 originalFoodPosition;
    private Vector2 originalCan2Position;

    [SerializeField] private Image chatImage;
    [SerializeField] private Sprite catSpriteNormal;
    [SerializeField] private Sprite catSpriteAngry;
    
    [SerializeField] private Image tunaCanImage;
    [SerializeField] private Sprite tunaCanOpen;
    [SerializeField] private Sprite tunaCanClosed;

    private void Awake()
    {
        if (lidRect != null) originalLidPosition = lidRect.anchoredPosition;
        if (tunaFoodRect != null) originalFoodPosition = tunaFoodRect.anchoredPosition;
        if (canPhase2Rect != null) originalCan2Position = canPhase2Rect.anchoredPosition;
        
        if (chatImage != null) chatImage.sprite = catSpriteAngry;
        
        
    }

    private void OnEnable()
    {
        if (tunaCanImage != null) tunaCanImage.sprite = tunaCanClosed;
        
        currentPhase = Phase.Opening;
        currentDragProgress = 0f;

        if (canPhase1Rect != null) canPhase1Rect.gameObject.SetActive(true);
        if (lidRect != null)
        {
            lidRect.gameObject.SetActive(true);
            lidRect.localEulerAngles = Vector3.zero;
            lidRect.anchoredPosition = originalLidPosition;
        }

        if (canPhase2Rect != null)
        {
            canPhase2Rect.gameObject.SetActive(false);
            canPhase2Rect.anchoredPosition = originalCan2Position;
            canPhase2Rect.localEulerAngles = Vector3.zero;
        }

        if (tunaFoodRect != null)
        {
            tunaFoodRect.gameObject.SetActive(false);
            tunaFoodRect.anchoredPosition = originalFoodPosition;
        }

        if (statusText != null)
        {
            statusText.text = "GLISSE VERS LA GAUCHE POUR OUVRIR !";
            statusText.color = Color.white;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentPhase == Phase.Done) return;

        if (currentPhase == Phase.Opening)
        {
            if (eventData.delta.x < 0)
            {
                currentDragProgress += Mathf.Abs(eventData.delta.x) * dragSensitivity;
                UpdateLidRotation();

                if (currentDragProgress >= requiredDragDistance)
                {
                    TransitionToPouring();
                }
            }
        }
        else if (currentPhase == Phase.Pouring)
        {
            float newY = canPhase2Rect.anchoredPosition.y + eventData.delta.y;
            if (newY <= originalCan2Position.y) 
            {
                canPhase2Rect.anchoredPosition = new Vector2(canPhase2Rect.anchoredPosition.x, newY);
            }

            canPhase2Rect.localEulerAngles = new Vector3(0, 0, Mathf.Sin(Time.time * 40f) * 10f);

            if (originalCan2Position.y - canPhase2Rect.anchoredPosition.y >= pullDownThreshold)
            {
                CompleteMiniGame();
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentPhase == Phase.Pouring)
        {
            canPhase2Rect.DOAnchorPosY(originalCan2Position.y, 0.3f).SetEase(Ease.OutBack);
            canPhase2Rect.DOLocalRotate(Vector3.zero, 0.3f);
        }
    }

    private void UpdateLidRotation()
    {
        if (lidRect == null) return;
        float progress = Mathf.Clamp01(currentDragProgress / requiredDragDistance);
        lidRect.localEulerAngles = new Vector3(0, 0, progress * 360f);
    }

    private void TransitionToPouring()
    {
        currentPhase = Phase.Done; 
        
        tunaCanImage.sprite = tunaCanOpen;

        lidRect.DOAnchorPosY(lidRect.anchoredPosition.y + 300f, 0.4f).SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                lidRect.gameObject.SetActive(false);
                canPhase1Rect.gameObject.SetActive(false); 
                canPhase2Rect.gameObject.SetActive(true); 

                currentPhase = Phase.Pouring;
                
                if (statusText != null)
                {
                    statusText.text = "TIRE LA BOÎTE VERS LE BAS !";
                    statusText.color = new Color(1f, 0.6f, 0f);
                }
            });
    }

    private void CompleteMiniGame()
    {
        currentPhase = Phase.Done;

        canPhase2Rect.localEulerAngles = Vector3.zero;

        if (statusText != null)
        {
            chatImage.sprite = catSpriteNormal;
            statusText.text = "LE CHAT EST SERVI !";
            statusText.color = Color.green;
        }

        tunaFoodRect.gameObject.SetActive(true);
        tunaFoodRect.anchoredPosition = canPhase2Rect.anchoredPosition; 
        
        tunaFoodRect.DOAnchorPosY(tunaFoodRect.anchoredPosition.y - 300f, 0.4f).SetEase(Ease.InCubic);
        
        canPhase2Rect.DOAnchorPosY(originalCan2Position.y + 100f, 0.3f).SetEase(Ease.OutBack);

        CrisisManager.Instance?.ResolveCrisis(minigameID);

        DOVirtual.DelayedCall(1.2f, () =>
        {
            MinigameManager.Instance?.CloseMinigame(minigameID);
        });
    }
}