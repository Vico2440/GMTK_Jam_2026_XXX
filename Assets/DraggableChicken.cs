using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class DraggableChicken : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public OvenMiniGame manager;
    [HideInInspector] public bool isDraggable = false;
    
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        originalPosition = rectTransform.anchoredPosition;
    }

    public void ResetChicken()
    {
        rectTransform.anchoredPosition = originalPosition;
        isDraggable = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        
        transform.DOScale(Vector3.one * 1.2f, 0.2f);
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
        transform.DOScale(Vector3.one, 0.2f);

        float distance = Vector2.Distance(originalPosition, rectTransform.anchoredPosition);

        if (distance > 200f)
        {
            isDraggable = false;
            manager.WinGame();
        }
        else
        {
            rectTransform.DOAnchorPos(originalPosition, 0.3f).SetEase(Ease.OutBack);
        }
    }
}
