using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;

public class CircuitBreakerMiniGame : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Doit être EXACTEMENT le même nom que dans le MinigameManager")]
    [SerializeField] private string minigameID = "Disjoncteur";
    
    [Tooltip("Nombre de boutons désactivés la 1ère fois")]
    [SerializeField] private int initialBrokenSwitches = 3;

    private int currentDifficulty;

    [Header("UI Elements")]
    [SerializeField] private Button[] breakerButtons; 
    
    [Header("Sprites")]
    [SerializeField] private Sprite spriteOn;
    [SerializeField] private Sprite spriteOff;
    
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI statusText;

    private bool[] isBroken;
    private int remainingToFix = 0;
    private bool isCompleted = false;

    private void Awake()
    {
        currentDifficulty = initialBrokenSwitches;
        isBroken = new bool[breakerButtons.Length];

        for (int i = 0; i < breakerButtons.Length; i++)
        {
            int index = i; 
            breakerButtons[i].onClick.RemoveAllListeners();
            breakerButtons[i].onClick.AddListener(() => OnButtonClicked(index));
            breakerButtons[i].transition = Selectable.Transition.None;
        }
    }

    private void OnEnable()
    {
        isCompleted = false;

        for (int i = 0; i < breakerButtons.Length; i++)
        {
            isBroken[i] = false;
            breakerButtons[i].image.sprite = spriteOn;
        }

        int brokenCount = Mathf.Min(currentDifficulty, breakerButtons.Length);
        remainingToFix = brokenCount;

        List<int> availableIndices = new List<int>();
        for (int i = 0; i < breakerButtons.Length; i++) availableIndices.Add(i);

        for (int i = 0; i < brokenCount; i++)
        {
            int randomIndex = Random.Range(0, availableIndices.Count);
            int buttonIndex = availableIndices[randomIndex];
            availableIndices.RemoveAt(randomIndex);

            isBroken[buttonIndex] = true;
            breakerButtons[buttonIndex].image.sprite = spriteOff;
        }

        if (statusText != null)
        {
            statusText.text = $"RESET THE {remainingToFix} CIRCUIT BREAKERS !";
            statusText.color = Color.red;
        }
    }

    private void OnButtonClicked(int index)
    {
        if (isCompleted || !isBroken[index]) return;
        
        SoundManager.Instance?.PlaySound("SwitchDis");

        isBroken[index] = false;
        breakerButtons[index].image.sprite = spriteOn;
        remainingToFix--;

        breakerButtons[index].transform.DOPunchScale(Vector3.one * 0.2f, 0.15f, vibrato: 10);

        if (statusText != null)
            statusText.text = $"REMAINING {remainingToFix} !";

        if (remainingToFix <= 0)
        {
            CompleteMiniGame();
        }
    }

    private void CompleteMiniGame()
    {
        SoundManager.Instance.StopAllSounds();
        
        isCompleted = true;
        
        int randomIncrease = Random.Range(1, 3); // Tire 1 ou 2
        currentDifficulty += randomIncrease; 

        if (statusText != null)
        {
            statusText.text = "POWER RESTORED!";
            statusText.color = Color.green;
        }

        CrisisManager.Instance?.ResolveCrisis(minigameID);

        DOVirtual.DelayedCall(1.0f, () =>
        {
            MinigameManager.Instance?.CloseMinigame(minigameID);
        });
    }
}