using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class DraggableAspirin : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public AspirinMiniGame manager;
    
    [Header("Cibles")]
    public RectTransform topOpeningZone; 
    public RectTransform bottomOfWater; 

    [Header("Particules (Bulles)")]
    public GameObject bubblePrefab;
    public RectTransform glassContainer;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private bool isDraggable = true;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    // Appelé automatiquement au moment où le manager le spawn dans la scène
    public void Setup(AspirinMiniGame mainManager, RectTransform opening, RectTransform bottom, GameObject bubble, RectTransform glass)
    {
        manager = mainManager;
        topOpeningZone = opening;
        bottomOfWater = bottom;
        bubblePrefab = bubble;
        glassContainer = glass;
        
        // On mémorise la position de spawn où il vient d'apparaître
        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        transform.DOScale(Vector3.one * 1.3f, 0.2f);
        canvasGroup.blocksRaycasts = false; 
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        rectTransform.anchoredPosition += eventData.delta / manager.mainCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        canvasGroup.blocksRaycasts = true;

        // Détection propre avec le New Input System
        bool isInsideOpening = RectTransformUtility.RectangleContainsScreenPoint(
            topOpeningZone, 
            eventData.position, 
            manager.mainCanvas.worldCamera
        );

        if (isInsideOpening)
        {
            isDraggable = false;
            
            rectTransform.DOMoveX(topOpeningZone.position.x, 0.1f);

            rectTransform.DOMoveY(bottomOfWater.position.y, 0.4f).SetEase(Ease.InQuad).OnComplete(() => 
            {
                float dissolveDuration = 0.8f;
                
                // L'aspirine rétrécit et se détruit
                transform.DOScale(Vector3.zero, dissolveDuration).OnComplete(() => 
                {
                    manager.OnAspirinDissolved(); // On prévient le manager !
                    Destroy(gameObject); 
                });

                StartCoroutine(SpawnBubblesRoutine(dissolveDuration));
            });
        }
        else
        {
            // Retour à la position de départ s'il a raté le verre
            transform.DOScale(Vector3.one, 0.2f);
            rectTransform.DOAnchorPos(originalPosition, 0.3f).SetEase(Ease.OutBack);
        }
    }

    private IEnumerator SpawnBubblesRoutine(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            CreateSingleBubble();
            float waitTime = Random.Range(0.05f, 0.12f);
            elapsed += waitTime;
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void CreateSingleBubble()
    {
        if (bubblePrefab == null || glassContainer == null) return;

        GameObject bubble = Instantiate(bubblePrefab, glassContainer);
        RectTransform bubbleRT = bubble.GetComponent<RectTransform>();

        bubbleRT.position = rectTransform.position + new Vector3(Random.Range(-25f, 25f), Random.Range(-10f, 10f), 0);
        bubbleRT.localScale = Vector3.one * Random.Range(0.5f, 1.1f);

        float floatTime = Random.Range(0.6f, 1.0f);
        float floatHeight = Random.Range(100f, 220f); 
        
        bubbleRT.DOMoveY(bubbleRT.position.y + floatHeight, floatTime).SetEase(Ease.OutQuad);
        bubbleRT.DOMoveX(bubbleRT.position.x + Random.Range(-15f, 15f), floatTime).SetEase(Ease.InOutSine);

        Image img = bubble.GetComponent<Image>();
        if (img != null)
        {
            img.DOFade(0f, floatTime).SetEase(Ease.InExpo).OnComplete(() => Destroy(bubble));
        }
        else
        {
            Destroy(bubble, floatTime);
        }
    }
}