using UnityEngine;
using TMPro;

public class SurvivalTimer : MonoBehaviour
{
    public static SurvivalTimer Instance;

    [Header("UI Elements")]
    [Tooltip("Le texte qui s'affiche sur ton bel écran de Game Over")]
    public TextMeshProUGUI gameOverTimeText;

    private float survivalTime = 0f;
    private bool isPlayerAlive = true;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (!isPlayerAlive || Time.timeScale == 0f) return;

        survivalTime += Time.deltaTime;
    }


    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }


    public void StopTimerAndDisplay()
    {
        isPlayerAlive = false;

        if (gameOverTimeText != null)
        {
            gameOverTimeText.text = "Survival time : " + FormatTime(survivalTime);
        }
    }
}