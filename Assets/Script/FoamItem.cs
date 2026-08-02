using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.InputSystem; 

public class FoamItem : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    [HideInInspector] public WashingMachineMiniGame manager;
    private bool isWiped = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            Wipe();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Wipe();
        }
    }

    private void Wipe()
    {
        if (isWiped) return;
        isWiped = true;
        
        transform.DOScale(Vector3.zero, 0.15f).SetEase(Ease.InBack).OnComplete(() => {
            manager.OnFoamWiped();
            Destroy(gameObject); 
        });
    }
}