using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Pour animer la barre en douceur

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance;

    [Header("Configuration du Temps")]
    [SerializeField] private float maxTime = 100f; 
    private float currentTime;
    private bool isTimerRunning = false;

    [Header("UI Elements")]
    [SerializeField] private Image timerBarFill; 

    [Header("Animation DOTween (Optionnel)")]
    [SerializeField] private bool useSmoothBar = true;
    [SerializeField] private float barAnimDuration = 0.2f;
    private Tween barTween;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        ResetTimer();
        StartTimer();
    }

    private void Update()
    {
        if (!isTimerRunning) return;

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateUI();

            if (currentTime <= 0)
            {
                currentTime = 0;
                TimerFinished();
            }
        }
    }


    public void StartTimer()
    {
        isTimerRunning = true;
    }

    public void PauseTimer()
    {
        isTimerRunning = false;
    }

    public void ResetTimer()
    {
        currentTime = maxTime;
        UpdateUI(instant: true);
    }

    /// <summary>
    /// Ajoute du temps au timer (ex: bonus de mini-jeu)
    /// </summary>
    public void AddTime(float seconds)
    {
        currentTime = Mathf.Clamp(currentTime + seconds, 0f, maxTime);
        UpdateUI();

        if (timerBarFill != null)
        {
            timerBarFill.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
        }
    }

    /// <summary>
    /// Retire du temps au timer (ex: dégâts, piège, mini-jeu raté)
    /// </summary>
    public void RemoveTime(float seconds)
    {
        currentTime = Mathf.Clamp(currentTime - seconds, 0f, maxTime);
        UpdateUI();

        if (timerBarFill != null)
        {
            timerBarFill.transform.DOShakePosition(0.3f, strength: 5f, vibrato: 20);
        }

        if (currentTime <= 0)
        {
            TimerFinished();
        }
    }

    private void UpdateUI(bool instant = false)
    {
        if (timerBarFill == null) return;

        float fillAmount = currentTime / maxTime;

        if (useSmoothBar && !instant)
        {
            if (barTween != null && barTween.IsActive()) barTween.Kill();
            barTween = timerBarFill.DOFillAmount(fillAmount, barAnimDuration);
        }
        else
        {
            timerBarFill.fillAmount = fillAmount;
        }
    }

    private void TimerFinished()
    {
        isTimerRunning = false;
        Debug.Log("Game Over");
        
    }

    public float GetCurrentTime() => currentTime;
    public bool IsTimerRunning() => isTimerRunning;
}