using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class PresenceManager : MonoBehaviour
{
    public static PresenceManager Instance;

    [Header("Réglages de la Jauge")]
    [SerializeField] private float maxPresence = 100f;
    [SerializeField] private float currentPresence = 100f;

    [Header("Vitesses (Points par seconde)")]
    [SerializeField] private float fillSpeed = 25f;  
    [SerializeField] private float drainSpeed = 10f;  

    [Header("UI")]
    [SerializeField] private Image presenceBarFill;  
    
    [Header("Game Over")]
    public UnityEvent onGameOver;
    
    [SerializeField] private string textGameOver = "You are fired !";

    private bool isAtPC = false;
    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (isGameOver) return;

        if (isAtPC)
        {
            currentPresence = Mathf.Clamp(currentPresence + fillSpeed * Time.deltaTime, 0f, maxPresence);
        }
        else
        {
            currentPresence -= drainSpeed * Time.deltaTime;

            if (currentPresence <= 0f)
            {
                currentPresence = 0f;
                TriggerGameOver();
            }
        }
        
        

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (presenceBarFill != null)
        {
            presenceBarFill.fillAmount = currentPresence / maxPresence;
        }
    }

    /// <summary>
    /// Appelé par la zone du PC pour dire si le joueur est devant ou pas
    /// </summary>
    public void SetPlayerAtPC(bool state)
    {
        isAtPC = state;

        if (isAtPC && presenceBarFill != null)
        {
            presenceBarFill.transform.DOPunchScale(Vector3.one * 0.05f, 0.2f);
        }
    }

    private void TriggerGameOver()
    {
        isGameOver = true;
        
        if (presenceBarFill != null)
        {
            presenceBarFill.transform.DOShakePosition(0.5f, strength: 10f);
        }
        
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(textGameOver);
        }
        
        SurvivalTimer.Instance.StopTimerAndDisplay();
        
        onGameOver?.Invoke();

    }

    public void AddPresence(float amount) => currentPresence = Mathf.Clamp(currentPresence + amount, 0f, maxPresence);
    public void RemovePresence(float amount) => currentPresence = Mathf.Clamp(currentPresence - amount, 0f, maxPresence);

    public bool IsAtPC() => isAtPC;
    public float GetPresenceRatio() => currentPresence / maxPresence;
}
