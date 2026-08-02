using UnityEngine;
using UnityEngine.UI;

public class MinigameTimerBar : MonoBehaviour
{
    [Header("Configuration")]
    public string crisisID;
    public Image fillImage;

    [Header("Couleurs")]
    public bool useColors = true;
    public Color highTimeColor = Color.green;
    public Color mediumTimeColor = Color.yellow;
    public Color lowTimeColor = Color.red;

    private void Update()
    {
        if (CrisisManager.Instance == null || fillImage == null) return;

        CrisisManager.CrisisData data = CrisisManager.Instance.GetCrisisData(crisisID);
        
        if (data != null && data.isBeingPlayed)
        {
            float ratio = data.timeRemaining / data.currentTimeLimit;
            
            fillImage.fillAmount = ratio;

            if (useColors)
            {
                if (ratio > 0.5f)
                {
                    fillImage.color = Color.Lerp(mediumTimeColor, highTimeColor, (ratio - 0.5f) * 2f);
                }
                else
                {
                    fillImage.color = Color.Lerp(lowTimeColor, mediumTimeColor, ratio * 2f);
                }
            }
        }
    }

    private void OnEnable()
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = 1f;
            if (useColors) fillImage.color = highTimeColor;
        }
    }
}