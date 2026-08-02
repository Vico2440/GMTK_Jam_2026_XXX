using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BoxWifiMiniGame : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private int requiredClicks = 10;    
    [SerializeField] private int difficultyIncrease = 3;  
    private int currentClicks = 0;

    [Header("UI Elements")]
    [SerializeField] private Image boxImage;             
    [SerializeField] private Image progressBarFill;       
    [SerializeField] private TextMeshProUGUI statusText;  

    [Header("Effets DOTween")]
    [SerializeField] private float jumpPower = 30f;       
    [SerializeField] private float jumpDuration = 0.15f;   
    [SerializeField] private float punchScale = 0.15f;    

    private bool isCompleted = false;
    private Vector3 originalBoxPosition;
    private Tween activeJumpTween;
    
    [SerializeField] private Sprite wifiIconRed;
    [SerializeField] private Sprite wifiIconGreen;
    
    [SerializeField] private Image wifiIcon;

    private void OnEnable()
    {
        wifiIcon.sprite = wifiIconRed;
        
        currentClicks = 0;
        isCompleted = false;

        if (boxImage != null)
        {
            originalBoxPosition = boxImage.rectTransform.anchoredPosition;
        }

        if (statusText != null)
        {
            statusText.text = "NO CONNECTION! CLICK TO RESTART!";
            statusText.color = Color.red;
        }

        UpdateUI();
    }

    /// <summary>
    /// À relier au bouton de la Box Wi-Fi (OnClick)
    /// </summary>
    public void OnClickBox()
    {
        if (isCompleted) return;
        
        int random = Random.Range(1, 3);
        
        SoundManager.Instance.PlaySound("Wifi" + random);

        currentClicks++;

        AnimateBoxJump();

        UpdateUI();

        if (currentClicks >= requiredClicks)
        {
            CompleteMiniGame();
        }
    }

    private void AnimateBoxJump()
    {
        if (boxImage == null) return;

        if (activeJumpTween != null && activeJumpTween.IsActive())
        {
            activeJumpTween.Kill();
            boxImage.rectTransform.anchoredPosition = originalBoxPosition;
        }

        Sequence jumpSeq = DOTween.Sequence();
        jumpSeq.Append(boxImage.rectTransform.DOAnchorPosY(originalBoxPosition.y + jumpPower, jumpDuration * 0.5f).SetEase(Ease.OutQuad));
        jumpSeq.Append(boxImage.rectTransform.DOAnchorPosY(originalBoxPosition.y, jumpDuration * 0.5f).SetEase(Ease.InQuad));
        
        boxImage.transform.DOPunchScale(Vector3.one * punchScale, jumpDuration, vibrato: 5);

        activeJumpTween = jumpSeq;
    }

    private void UpdateUI()
    {
        if (progressBarFill != null)
        {
            progressBarFill.fillAmount = (float)currentClicks / requiredClicks;
        }
    }

    private void CompleteMiniGame()
    {
        SoundManager.Instance.StopAllSounds();
        
        isCompleted = true;
        requiredClicks += difficultyIncrease;

        CrisisManager.Instance?.ResolveCrisis("BoxWifi");

        if (statusText != null)
        {
            wifiIcon.sprite = wifiIconGreen;
            statusText.text = "SIGNAL RESTORED!";
            statusText.color = Color.green;
        }

        boxImage.transform.DOPunchScale(Vector3.one * 0.4f, 0.4f, vibrato: 10)
            .OnComplete(() =>
            {
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    MinigameManager.Instance?.CloseMinigame("BoxWifi");
                });
            });
    }
}