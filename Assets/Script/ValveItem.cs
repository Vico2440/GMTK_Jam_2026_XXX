using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class ValveItem : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [HideInInspector] public SinkMiniGame manager;
    
    [Tooltip("Le nombre de tours complets (360°) nécessaires pour la fermer")]
    public float requiredTurns = 2f; 
    
    private RectTransform rectTransform;
    private Image valveImage;
    private bool isClosed = false;
    
    private float previousAngle;
    private float totalRotation = 0f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        valveImage = GetComponent<Image>();
    }

    public void ResetValve()
    {
        if (valveImage != null)
        {
            isClosed = false;
            totalRotation = 0f;
        
            requiredTurns = Random.Range(1.5f, 3f);
        
            transform.localRotation = Quaternion.identity;
            valveImage.color = Color.white;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isClosed) return;
        
        Vector2 center = RectTransformUtility.WorldToScreenPoint(manager.mainCanvas.worldCamera, rectTransform.position);
        Vector2 dir = eventData.position - center;
        previousAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isClosed) return;

        Vector2 center = RectTransformUtility.WorldToScreenPoint(manager.mainCanvas.worldCamera, rectTransform.position);
        Vector2 dir = eventData.position - center;
        float currentAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        float deltaAngle = Mathf.DeltaAngle(previousAngle, currentAngle);

        rectTransform.Rotate(0, 0, deltaAngle);

        totalRotation += Mathf.Abs(deltaAngle);

        previousAngle = currentAngle;

        if (totalRotation >= requiredTurns * 360f)
        {
            CloseValve();
        }
    }

    private void CloseValve()
    {
        isClosed = true;
        
        // Feedback : la valve se bloque
        transform.DOPunchScale(Vector3.one * 0.2f, 0.3f);
        valveImage.DOColor(new Color(0.8f, 0.8f, 0.8f), 0.3f);

        manager.OnValveClosed();
    }
}