using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class OvenMiniGame : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private string minigameID = "Four";
    
    [Tooltip("Glisse ici le composant Canvas principal de ton mini-jeu")]
    public Canvas mainCanvas; 

    [Header("Sprites du Four")]
    [Tooltip("L'image de fond qui va changer de sprite")]
    [SerializeField] private Image ovenMainImage;
    [SerializeField] private Sprite ovenClosedSprite;
    [SerializeField] private Sprite ovenOpenSprite;

    [Header("UI Elements")]
    [SerializeField] private Button knobButton;
    [Tooltip("L'objet de la porte séparée qu'on va désactiver")]
    [SerializeField] private RectTransform ovenDoor; 
    [SerializeField] private DraggableChicken chicken;
    [SerializeField] private TextMeshProUGUI statusText;

    private bool isOvenOff = false;
    private bool isCompleted = false;

    private void Awake()
    {
        chicken.manager = this;
        
        knobButton.onClick.RemoveAllListeners();
        knobButton.onClick.AddListener(TurnOffOven);
        knobButton.transition = Selectable.Transition.None;
    }

    private void OnEnable()
    {
        isCompleted = false;
        isOvenOff = false;
        
        chicken.ResetChicken();
        
        if (ovenMainImage != null && ovenClosedSprite != null)
        {
            ovenMainImage.sprite = ovenClosedSprite;
        }

        if (ovenDoor != null)
        {
            ovenDoor.gameObject.SetActive(true);
        }

        if (statusText != null)
        {
            statusText.text = "ÉTEINS LE FOUR !";
            statusText.color = Color.red;
        }
    }

    private void TurnOffOven()
    {
        if (isOvenOff || isCompleted) return;
        isOvenOff = true;

        knobButton.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f);

        if (statusText != null)
        {
            statusText.text = "TAKE THE CHICKEN OUT OF THE OVEN!";
            statusText.color = new Color(1f, 0.6f, 0f); 
        }

        if (ovenMainImage != null && ovenOpenSprite != null)
        {
            ovenMainImage.sprite = ovenOpenSprite;
        }

        if (ovenDoor != null)
        {
            ovenDoor.gameObject.SetActive(false);
        }
            
        chicken.isDraggable = true;
    }

    public void WinGame()
    {
        if (isCompleted) return;
        isCompleted = true;

        if (statusText != null)
        {
            statusText.text = "CHICKEN SAVED !";
            statusText.color = Color.green;
        }

        CrisisManager.Instance?.ResolveCrisis(minigameID);

        DOVirtual.DelayedCall(1.0f, () =>
        {
            MinigameManager.Instance?.CloseMinigame(minigameID);
        });
    }
}